<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" encoding="utf-8" indent="yes"/>

	<xsl:template match="/">
		<html>
			<head>
				<title>Успішність студентів</title>
				<style>
					body {
					font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
					margin: 20px;
					background-color: #f5f5f5;
					}
					h1 {
					color: #2c3e50;
					text-align: center;
					margin-bottom: 30px;
					}
					table {
					width: 100%;
					border-collapse: collapse;
					background-color: white;
					box-shadow: 0 2px 4px rgba(0,0,0,0.1);
					}
					th {
					background-color: #3498db;
					color: white;
					padding: 12px;
					text-align: left;
					font-weight: bold;
					}
					td {
					padding: 10px;
					border-bottom: 1px solid #ddd;
					}
					tr:hover {
					background-color: #f8f9fa;
					}
					.high-grade {
					color: #27ae60;
					font-weight: bold;
					}
					.medium-grade {
					color: #f39c12;
					}
					.low-grade {
					color: #e74c3c;
					}
					.stats {
					margin-top: 20px;
					padding: 15px;
					background-color: #ecf0f1;
					border-radius: 5px;
					}
				</style>
			</head>
			<body>
				<h1>Успішність студентів університету</h1>

				<table>
					<thead>
						<tr>
							<th>ПІБ</th>
							<th>Факультет</th>
							<th>Кафедра</th>
							<th>Предмет</th>
							<th>Оцінка</th>
							<th>Семестр</th>
							<th>Курс</th>
						</tr>
					</thead>
					<tbody>
						<xsl:apply-templates select="//Student">
							<xsl:sort select="Grade" data-type="number" order="descending"/>
						</xsl:apply-templates>
					</tbody>
				</table>

				<div class="stats">
					<h3>Статистика:</h3>
					<p>
						Всього студентів: <strong>
							<xsl:value-of select="count(//Student)"/>
						</strong>
					</p>
					<p>
						Середній бал: <strong>
							<xsl:value-of select="format-number(sum(//Student/Grade) div count(//Student), '##.##')"/>
						</strong>
					</p>
					<p>
						Студентів з балом 90+: <strong>
							<xsl:value-of select="count(//Student[Grade &gt;= 90])"/>
						</strong>
					</p>
				</div>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="Student">
		<tr>
			<td>
				<xsl:value-of select="FullName"/>
			</td>
			<td>
				<xsl:choose>
					<xsl:when test="@faculty">
						<xsl:value-of select="@faculty"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="Faculty"/>
					</xsl:otherwise>
				</xsl:choose>
			</td>
			<td>
				<xsl:choose>
					<xsl:when test="@department">
						<xsl:value-of select="@department"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="Department"/>
					</xsl:otherwise>
				</xsl:choose>
			</td>
			<td>
				<xsl:value-of select="Subject"/>
			</td>
			<td>
				<xsl:attribute name="class">
					<xsl:choose>
						<xsl:when test="Grade &gt;= 90">high-grade</xsl:when>
						<xsl:when test="Grade &gt;= 75">medium-grade</xsl:when>
						<xsl:otherwise>low-grade</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
				<xsl:value-of select="Grade"/>
			</td>
			<td>
				<xsl:choose>
					<xsl:when test="@semester">
						<xsl:value-of select="@semester"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="Semester"/>
					</xsl:otherwise>
				</xsl:choose>
			</td>
			<td>
				<xsl:choose>
					<xsl:when test="@course">
						<xsl:value-of select="@course"/>
					</xsl:when>
					<xsl:otherwise>-</xsl:otherwise>
				</xsl:choose>
			</td>
		</tr>
	</xsl:template>

</xsl:stylesheet>