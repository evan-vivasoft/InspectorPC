# 
# Usage:
#   runSourceMonitor.ps1 DirectoryOfSourceMonitor DirectoryForImageFiles ProjectFileName
#
# Outputs:
#   <DirectoryForImageFiles>\<guid>.jpg - Kiviat file
#   temp\sm_dump.xml - SourceMonitor dump
#   temp\sm_kiviat.xml - XML snippet containing filename of kiviat file

$SourceMonitorRoot = $args[0] 
$SourceMonitorExe = $SourceMonitorRoot + "\SourceMonitor.exe"
$Bmp2JpgExe = $SourceMonitorRoot + "\Contributions\Daniele_Teti\bmp2jpg.exe"
$htmldir = $args[1]
$projectFileName = $args[2]

#XSLT conversion from from http://huddledmasses.org/convert-xml-with-xslt-in-powershell/
function Convert-WithXslt($originalXmlFilePath, $xslFilePath, $outputFilePath) 
{
   ## Simplistic error handling
   $xslFilePath = resolve-path $xslFilePath
   if( -not (test-path $xslFilePath) ) { throw "Can't find the XSL file" } 
   $originalXmlFilePath = resolve-path $originalXmlFilePath
   if( -not (test-path $originalXmlFilePath) ) { throw "Can't find the XML file" } 
   #$outputFilePath = resolve-path $outputFilePath
   #if( -not (test-path (split-path $originalXmlFilePath)) ) { throw "Can't find the output folder" } 

   ## Get an XSL Transform object (try for the new .Net 3.5 version first)
   $EAP = $ErrorActionPreference
   $ErrorActionPreference = "SilentlyContinue"
   $script:xslt = new-object system.xml.xsl.xslcompiledtransform
   trap [System.Management.Automation.PSArgumentException] 
   {  # no 3.5, use the slower 2.0 one
      $ErrorActionPreference = $EAP
      $script:xslt = new-object system.xml.xsl.xsltransform
   }
   $ErrorActionPreference = $EAP
   
   ## load xslt file
   $xslt.load( $xslFilePath )
     
   ## transform 
   $xslt.Transform( $originalXmlFilePath, $outputFilePath )
}


# ensure output dir exists.
if ((Test-Path -path $htmldir) -ne $True)
{
	Write-Host $htmldir + " doesnt exist!"
	exit
}

$localProjectFileName = "buildserver_$projectFileName"

# don't overwrite the smproj that's in source control
if ((Test-Path -path .\$localProjectFileName ) -ne $True)
{
  copy $projectFileName $localProjectFileName  
}

#minimize windows so they wont appear in the captured bitmap
$shell = New-Object -ComObject "Shell.Application"
$shell.minimizeall()

Start-Process -FilePath "$SourceMonitorExe" -ArgumentList "/C CreateCheckpoint_$localProjectFileName.xml" -Wait

Convert-WithXslt ".\temp\sm_dump_$localProjectFileName.xml" SourceMonitorSummaryGeneration.xsl ".\temp\sm_dump_summary_$localProjectFileName.xml"

# convert created kiviat file (as specified in CreateCheckpoint.xml) to JPG
Start-Process -FilePath "$Bmp2JpgExe" -ArgumentList "$PWD\temp\sm_kiviat_$localProjectFileName.bmp" -Wait

# invent a random image filename
$guid = [guid]::NewGuid().ToString("N") + ".jpg"
$kiviatOut = $htmldir + "\" + $guid

# move image to web directory
move-item "$PWD\temp\sm_kiviat_$localProjectFileName.jpg" $kiviatOut

# make a little HTML to be merged in the cruisecontrol log
$xml = @"
<?xml version="1.0" encoding="UTF-8" ?>
<smKiviat>
  <project name="$projectFileName"/>
  <kiviat name="$guid"/>
</smKiviat>
"@

# save XML to file.
$xml | Out-File -FilePath "temp\sm_kiviat_$localProjectFileName.xml"

#restore windows
$shell.undominimizeall()