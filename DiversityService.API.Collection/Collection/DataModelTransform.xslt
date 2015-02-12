<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:ssdl="http://schemas.microsoft.com/ado/2009/11/edm/ssdl"
                xmlns:edmx="http://schemas.microsoft.com/ado/2009/11/edmx"
                xmlns:edm="http://schemas.microsoft.com/ado/2009/11/edm"
                xmlns:annotation="http://schemas.microsoft.com/ado/2009/02/edm/annotation"
                xmlns:mapping="http://schemas.microsoft.com/ado/2009/11/mapping/cs">
    <xsl:output method="xml" indent="yes" />

    <!--Ensure blank lines aren't left when we remove fields.-->
    <xsl:strip-space  elements="*" />

    <xsl:template match="@*|*|processing-instruction()|comment()">
        <xsl:call-template name="CopyDetails" />
    </xsl:template>

    <xsl:template name="CopyDetails">
        <xsl:copy>
            <xsl:apply-templates select="@*|*|text()|processing-instruction()|comment()" />
        </xsl:copy>
    </xsl:template>

    <!-- Add the ConcurrencyAttribute if it doesn't exist, otherwise update it if it does -->
    <xsl:template name="AddConcurrencyAttribute">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()" />
            <xsl:attribute name="ConcurrencyMode">Fixed</xsl:attribute>
        </xsl:copy>
    </xsl:template>

    <!-- Remove Log Columns -->
    <!-- Prevents them from being set NULL on insert and consequently not being DB generated -->
    <xsl:template match="ssdl:EntityType/ssdl:Property | edm:EntityType/edm:Property | mapping:MappingFragment/mapping:ScalarProperty">
        <xsl:if test="not(starts-with(@Name,'Log'))">
            <xsl:call-template name="CopyDetails" />
        </xsl:if>
    </xsl:template>
</xsl:stylesheet>