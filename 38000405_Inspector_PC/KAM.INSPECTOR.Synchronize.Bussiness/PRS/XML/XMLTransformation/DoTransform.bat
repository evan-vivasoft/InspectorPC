@echo off

AltovaXML /xslt1 "T:\Source code\INSPECTOR.PC\Source Code\KAM.INSPECTOR.Synchronize.Bussiness\PRS\XML\XMLTransformation\MapToStationInformation.xslt" /in "T:\Source code\INSPECTOR.PC\Source Code\KAM.INSPECTOR.Synchronize.Bussiness\PRS\XML\XMLTransformation\Test files\StationInformationExport.xml" /out "T:\Source code\INSPECTOR.PC\Source Code\KAM.INSPECTOR.Synchronize.Bussiness\PRS\XML\XMLTransformation\Test files\StationInformation.xml" %*
IF ERRORLEVEL 1 EXIT/B %ERRORLEVEL%
