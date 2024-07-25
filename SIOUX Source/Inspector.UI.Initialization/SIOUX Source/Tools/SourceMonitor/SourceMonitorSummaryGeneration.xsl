<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  
  <!--- SourceMonitor Summary Xml File Generation created by Eden Ridgway -->
  <xsl:output method="xml"/>
  
  <xsl:template match="/">
    <xsl:apply-templates select="/sourcemonitor_metrics" />
  </xsl:template>
  
  <!-- Transform the results into a simpler more intuitive and summarised format -->
  <xsl:template match="sourcemonitor_metrics">
      <SourceMonitorComplexitySummary>
		<Language><xsl:value-of select="//project_language/text()"/></Language>
        <MostComplexMethods>
		  <xsl:choose>
			<xsl:when test="//project_language/text()='C#'">
				<xsl:call-template name="MostComplexMethodsCSharp"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="MostComplexMethodsCPP"/>
			</xsl:otherwise>
		  </xsl:choose>
        </MostComplexMethods>
        
        <MostDeeplyNestedCode>
		  <xsl:choose>
			<xsl:when test="//project_language/text()='C#'">
				<xsl:call-template name="MostDeeplyNestedCodeCSharp"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="MostDeeplyNestedCodeCPP"/>
			</xsl:otherwise>
		  </xsl:choose>		
        </MostDeeplyNestedCode>
        
      </SourceMonitorComplexitySummary>
  </xsl:template>
  
  <!-- Complexity Metrics -->
  <xsl:template name="MostComplexMethodsCSharp">
    <xsl:for-each select=".//file">
      <xsl:sort select="metrics/metric[@id='M10']" order="descending" data-type="number" />
      
      <xsl:choose>
        <xsl:when test="position() &lt; 16">
           <Method>
              <File><xsl:value-of select="@file_name"/></File>
              <Name><xsl:value-of select="metrics/metric[@id='M9']"/></Name>
              <Line><xsl:value-of select="metrics/metric[@id='M8']"/></Line>
              <Complexity><xsl:value-of select="metrics/metric[@id='M10']"/></Complexity>
           </Method>
        </xsl:when>
      </xsl:choose>
    </xsl:for-each>
  </xsl:template>

  <!-- Nesting Metrics -->
  <xsl:template name="MostDeeplyNestedCodeCSharp">
    <xsl:for-each select=".//file">
        <xsl:sort select="metrics/metric[@id='M12']" order="descending" data-type="text" />
      
        <xsl:choose>
          <xsl:when test="position() &lt; 16">
             <Block>
                <File><xsl:value-of select="@file_name"/></File>
                <Depth><xsl:value-of select="metrics/metric[@id='M12']"/></Depth>
                <Line><xsl:value-of select="metrics/metric[@id='M11']"/></Line>
             </Block>
          </xsl:when>
        </xsl:choose>
    </xsl:for-each>
  </xsl:template>  

  <!-- Complexity Metrics -->
  <xsl:template name="MostComplexMethodsCPP">
    <xsl:for-each select=".//file">
      <xsl:sort select="metrics/metric[@id='M9']" order="descending" data-type="number" />
      
      <xsl:choose>
        <xsl:when test="position() &lt; 16">
           <Method>
              <File><xsl:value-of select="@file_name"/></File>
              <Name><xsl:value-of select="metrics/metric[@id='M8']"/></Name>
              <Line><xsl:value-of select="metrics/metric[@id='M7']"/></Line>
              <Complexity><xsl:value-of select="metrics/metric[@id='M9']"/></Complexity>
           </Method>
        </xsl:when>
      </xsl:choose>
    </xsl:for-each>
  </xsl:template>

  <!-- Nesting Metrics -->
  <xsl:template name="MostDeeplyNestedCodeCPP">
    <xsl:for-each select=".//file">
        <xsl:sort select="metrics/metric[@id='M11']" order="descending" data-type="text" />
      
        <xsl:choose>
          <xsl:when test="position() &lt; 16">
             <Block>
                <File><xsl:value-of select="@file_name"/></File>
                <Depth><xsl:value-of select="metrics/metric[@id='M11']"/></Depth>
                <Line><xsl:value-of select="metrics/metric[@id='M10']"/></Line>
             </Block>
          </xsl:when>
        </xsl:choose>
    </xsl:for-each>
  </xsl:template>
  
</xsl:stylesheet>
