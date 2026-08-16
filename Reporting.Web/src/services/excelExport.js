function escapeXml(value) {
  return String(value ?? '')
    .replaceAll('&', '&amp;')
    .replaceAll('<', '&lt;')
    .replaceAll('>', '&gt;')
    .replaceAll('"', '&quot;')
}

function columnName(index) {
  let name = ''
  let current = index + 1

  while (current > 0) {
    const remainder = (current - 1) % 26
    name = String.fromCharCode(65 + remainder) + name
    current = Math.floor((current - remainder) / 26)
  }

  return name
}

function buildSheetXml(columns, rows) {
  const tableRows = [
    columns.map((column) => column.header),
    ...rows.map((row) => columns.map((column) => column.value(row))),
  ]

  const xmlRows = tableRows.map((values, rowIndex) => {
    const cells = values.map((value, columnIndex) => {
      const ref = `${columnName(columnIndex)}${rowIndex + 1}`
      if (typeof value === 'number' && Number.isFinite(value)) {
        return `<c r="${ref}" s="${rowIndex === 0 ? 1 : 0}"><v>${value}</v></c>`
      }

      return `<c r="${ref}" t="inlineStr" s="${rowIndex === 0 ? 1 : 0}"><is><t>${escapeXml(value)}</t></is></c>`
    }).join('')

    return `<row r="${rowIndex + 1}">${cells}</row>`
  }).join('')

  return `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
  <sheetData>${xmlRows}</sheetData>
</worksheet>`
}

function buildWorkbookXml(sheetName) {
  return `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
  <sheets><sheet name="${escapeXml(sheetName)}" sheetId="1" r:id="rId1"/></sheets>
</workbook>`
}

function buildStylesXml() {
  return `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<styleSheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">
  <fonts count="2"><font><sz val="11"/><name val="Calibri"/></font><font><b/><sz val="11"/><name val="Calibri"/></font></fonts>
  <fills count="2"><fill><patternFill patternType="none"/></fill><fill><patternFill patternType="gray125"/></fill></fills>
  <borders count="1"><border><left/><right/><top/><bottom/><diagonal/></border></borders>
  <cellStyleXfs count="1"><xf numFmtId="0" fontId="0" fillId="0" borderId="0"/></cellStyleXfs>
  <cellXfs count="2"><xf numFmtId="0" fontId="0" fillId="0" borderId="0" xfId="0"/><xf numFmtId="0" fontId="1" fillId="0" borderId="0" xfId="0"/></cellXfs>
</styleSheet>`
}

function crc32(bytes) {
  let crc = -1

  for (let index = 0; index < bytes.length; index += 1) {
    crc ^= bytes[index]
    for (let bit = 0; bit < 8; bit += 1) {
      crc = (crc >>> 1) ^ (0xedb88320 & -(crc & 1))
    }
  }

  return (crc ^ -1) >>> 0
}

function writeUint16(bytes, value) {
  bytes.push(value & 0xff, (value >>> 8) & 0xff)
}

function writeUint32(bytes, value) {
  bytes.push(value & 0xff, (value >>> 8) & 0xff, (value >>> 16) & 0xff, (value >>> 24) & 0xff)
}

function createZip(entries) {
  const encoder = new TextEncoder()
  const fileBytes = []
  const centralDirectory = []

  entries.forEach((entry) => {
    const nameBytes = encoder.encode(entry.name)
    const contentBytes = encoder.encode(entry.content)
    const checksum = crc32(contentBytes)
    const localOffset = fileBytes.length

    writeUint32(fileBytes, 0x04034b50)
    writeUint16(fileBytes, 20)
    writeUint16(fileBytes, 0)
    writeUint16(fileBytes, 0)
    writeUint16(fileBytes, 0)
    writeUint16(fileBytes, 0)
    writeUint32(fileBytes, checksum)
    writeUint32(fileBytes, contentBytes.length)
    writeUint32(fileBytes, contentBytes.length)
    writeUint16(fileBytes, nameBytes.length)
    writeUint16(fileBytes, 0)
    fileBytes.push(...nameBytes, ...contentBytes)

    writeUint32(centralDirectory, 0x02014b50)
    writeUint16(centralDirectory, 20)
    writeUint16(centralDirectory, 20)
    writeUint16(centralDirectory, 0)
    writeUint16(centralDirectory, 0)
    writeUint16(centralDirectory, 0)
    writeUint16(centralDirectory, 0)
    writeUint32(centralDirectory, checksum)
    writeUint32(centralDirectory, contentBytes.length)
    writeUint32(centralDirectory, contentBytes.length)
    writeUint16(centralDirectory, nameBytes.length)
    writeUint16(centralDirectory, 0)
    writeUint16(centralDirectory, 0)
    writeUint16(centralDirectory, 0)
    writeUint16(centralDirectory, 0)
    writeUint32(centralDirectory, 0)
    writeUint32(centralDirectory, localOffset)
    centralDirectory.push(...nameBytes)
  })

  const centralDirectoryOffset = fileBytes.length
  fileBytes.push(...centralDirectory)

  writeUint32(fileBytes, 0x06054b50)
  writeUint16(fileBytes, 0)
  writeUint16(fileBytes, 0)
  writeUint16(fileBytes, entries.length)
  writeUint16(fileBytes, entries.length)
  writeUint32(fileBytes, centralDirectory.length)
  writeUint32(fileBytes, centralDirectoryOffset)
  writeUint16(fileBytes, 0)

  return new Blob([new Uint8Array(fileBytes)], {
    type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
  })
}

export function downloadXlsx({ filename, sheetName, columns, rows }) {
  const entries = [
    {
      name: '[Content_Types].xml',
      content: `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
  <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
  <Default Extension="xml" ContentType="application/xml"/>
  <Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/>
  <Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>
  <Override PartName="/xl/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"/>
</Types>`,
    },
    {
      name: '_rels/.rels',
      content: `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/>
</Relationships>`,
    },
    {
      name: 'xl/workbook.xml',
      content: buildWorkbookXml(sheetName),
    },
    {
      name: 'xl/_rels/workbook.xml.rels',
      content: `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/>
  <Relationship Id="rId2" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/>
</Relationships>`,
    },
    {
      name: 'xl/worksheets/sheet1.xml',
      content: buildSheetXml(columns, rows),
    },
    {
      name: 'xl/styles.xml',
      content: buildStylesXml(),
    },
  ]

  const url = URL.createObjectURL(createZip(entries))
  const link = document.createElement('a')
  link.href = url
  link.download = filename
  document.body.appendChild(link)
  link.click()
  link.remove()
  URL.revokeObjectURL(url)
}
