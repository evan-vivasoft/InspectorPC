<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata" xmlns:xdt="http://www.w3.org/2005/xpath-datatypes" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:altova="http://www.altova.com">
	<xsl:output version="1.0" method="html" indent="no" encoding="UTF-8" doctype-public="-//W3C//DTD HTML 4.0 Transitional//EN" doctype-system="http://www.w3.org/TR/html4/loose.dtd"/>
	<xsl:param name="SV_OutputFormat" select="'HTML'"/>
	<xsl:variable name="XML" select="/"/>
	<xsl:template match="/">
		<html>
			<head>
				<title/>
			</head>
			<body>
				<xsl:for-each select="$XML">
					<h4>
						<span>
							<xsl:text>Resultaten:</xsl:text>
						</span>
					</h4>
					<xsl:for-each select="InspectionResultsData">
						<xsl:for-each select="InspectionResult">
							<table border="1">
								<tbody>
									<tr>
										<th>
											<span>
												<xsl:text>GDR Anlage</xsl:text>
											</span>
										</th>
										<td>
											<xsl:for-each select="PRSName">
												<xsl:apply-templates/>
											</xsl:for-each>
										</td>
									</tr>
									<tr>
										<th>
											<span>
												<xsl:text>Regelschiene</xsl:text>
											</span>
										</th>
										<td>
											<xsl:for-each select="GasControlLineName">
												<xsl:apply-templates/>
											</xsl:for-each>
										</td>
									</tr>
									<tr>
										<th>
											<span>
												<xsl:text>PLEXOR Manometer</xsl:text>
											</span>
										</th>
										<td>
											<xsl:for-each select="Measurement_Equipment">
												<xsl:for-each select="BT_Address">
													<xsl:apply-templates/>
												</xsl:for-each>
											</xsl:for-each>
											<br/>
											<xsl:for-each select="Measurement_Equipment">
												<xsl:for-each select="ID_DM1">
													<xsl:apply-templates/>
												</xsl:for-each>
											</xsl:for-each>
											<br/>
											<xsl:for-each select="Measurement_Equipment">
												<xsl:for-each select="ID_DM2">
													<xsl:apply-templates/>
												</xsl:for-each>
											</xsl:for-each>
											<br/>
										</td>
									</tr>
									<tr>
										<th>
											<span>
												<xsl:text>Prüfverfahren</xsl:text>
											</span>
										</th>
										<td>
											<xsl:for-each select="InspectionProcedure">
												<xsl:for-each select="Name">
													<xsl:apply-templates/>
												</xsl:for-each>
											</xsl:for-each>
											<span>
												<xsl:text>&#160;</xsl:text>
											</span>
											<xsl:for-each select="InspectionProcedure">
												<xsl:for-each select="Version">
													<xsl:apply-templates/>
												</xsl:for-each>
											</xsl:for-each>
										</td>
									</tr>
									<tr>
										<th>
											<span>
												<xsl:text>Datum</xsl:text>
											</span>
										</th>
										<td>
											<xsl:for-each select="DateTimeStamp">
												<xsl:for-each select="StartDate">
													<xsl:apply-templates/>
												</xsl:for-each>
											</xsl:for-each>
											<span>
												<xsl:text>&#160;</xsl:text>
											</span>
											<xsl:for-each select="DateTimeStamp">
												<xsl:for-each select="StartTime">
													<xsl:apply-templates/>
												</xsl:for-each>
											</xsl:for-each>
										</td>
									</tr>
								</tbody>
							</table>
							<br/>
							<table border="1">
								<thead>
									<tr>
										<th>
											<span>
												<xsl:text>Objekt Name</xsl:text>
											</span>
										</th>
										<th>
											<span>
												<xsl:text>Messpunkt</xsl:text>
											</span>
										</th>
										<th>
											<span>
												<xsl:text>Messwert</xsl:text>
											</span>
										</th>
										<th>
											<span>
												<xsl:text>Text</xsl:text>
											</span>
										</th>
									</tr>
								</thead>
								<tbody>
									<xsl:for-each select="Result">
										<tr>
											<td>
												<xsl:for-each select="ObjectName">
													<xsl:apply-templates/>
												</xsl:for-each>
											</td>
											<td>
												<xsl:for-each select="MeasurePoint">
													<xsl:apply-templates/>
												</xsl:for-each>
											</td>
											<td>
												<xsl:for-each select="MeasureValue">
													<xsl:apply-templates/>
												</xsl:for-each>
											</td>
											<td>
												<xsl:for-each select="Text">
													<xsl:apply-templates/>
												</xsl:for-each>
											</td>
										</tr>
									</xsl:for-each>
								</tbody>
							</table>
							<hr/>
							<br/>
							<br/>
						</xsl:for-each>
					</xsl:for-each>
					<br/>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>
