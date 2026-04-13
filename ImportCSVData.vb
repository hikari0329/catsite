Sub ImportCSVData()
    Dim wsCSV As Worksheet
    Dim wsSummary As Worksheet
    Dim csvFilePath As String
    Dim csvData As Variant
    Dim summaryData() As Variant
    Dim i As Long, j As Long
    Dim accountCol As Long, nameCol As Long
    
    ' 設定 CSV 檔案路徑
    csvFilePath = "C:\path\to\your\file.csv"
    
    ' 設定彙總工作表
    Set wsSummary = ThisWorkbook.Sheets("Summary")
    
    ' 讀取 CSV 檔案數據
    With CreateObject("Scripting.FileSystemObject")
        If .FileExists(csvFilePath) Then
            Set wsCSV = ThisWorkbook.Sheets.Add
            wsCSV.Name = "CSV Data"
            wsCSV.Cells.Clear
            csvData = ImportCSV(csvFilePath)
            wsCSV.Range("A1").Resize(UBound(csvData, 1), UBound(csvData, 2)).Value = csvData
        Else
            MsgBox "CSV 檔案不存在。"
            Exit Sub
        End If
    End With
    
    ' 在彙總工作表中建立表頭
    wsSummary.Cells.Clear
    wsSummary.Range("A1").Resize(1, 14).Value = Array("交易日期", "支出金額", "存入金額", "結存餘額", "終端", _
                                                       "交易時間", "交易摘要", "備註", "對方帳號", "櫃員", _
                                                       "帳號", "戶名")
    
    ' 尋找帳號和戶名的欄位
    For j = 1 To wsCSV.Cells(1, wsCSV.Columns.Count).End(xlToLeft).Column
        If wsCSV.Cells(1, j).Value Like "*帳號*" Then
            accountCol = j
        ElseIf wsCSV.Cells(1, j).Value Like "*戶名*" Then
            nameCol = j
        End If
    Next j
    
    ' 整理 CSV 檔案數據並寫入彙總工作表
    ReDim summaryData(1 To UBound(csvData, 1), 1 To 14)
    For i = 1 To UBound(csvData, 1)
        If Not IsDate(csvData(i, 1)) Then
            Continue For
        End If
        
        summaryData(i, 1) = csvData(i, 1)
        summaryData(i, 2) = csvData(i, 2)
        summaryData(i, 3) = csvData(i, 3)
        summaryData(i, 4) = csvData(i, 4)
        summaryData(i, 5) = csvData(i, 5)
        summaryData(i, 6) = csvData(i + 1, 1)
        summaryData(i, 7) = csvData(i + 1, 2)
        summaryData(i, 8) = csvData(i + 1, 3)
        summaryData(i, 9) = csvData(i + 1, 4)
        summaryData(i, 10) = csvData(i + 1, 5)
        summaryData(i, 11) = wsCSV.Cells(i + 1, accountCol).Value
        summaryData(i, 12) = wsCSV.Cells(i + 1, nameCol).Value
    Next i
    
    ' 寫入彙總數據到彙總工作表
    wsSummary.Range("A2").Resize(UBound(summaryData, 1), UBound(summaryData, 2)).Value = summaryData
    
    ' 刪除 CSV 工作表
    Application.DisplayAlerts = False
    wsCSV.Delete
    Application.DisplayAlerts = True
    
    MsgBox "資料已成功匯入到彙總工作表。"
End Sub

Function ImportCSV(csvFilePath As String) As Variant
    Dim csvData As String
    Dim rows As Variant
    Dim csvRows() As String
    Dim csvCols() As String
    Dim i As Long, j As Long
    
    ' 讀取 CSV 檔案內容
    Open csvFilePath For Input As #1
    csvData = Input(LOF(1), #1)
    Close #1
    
    ' 將 CSV 數據分割為行和列
    csvRows = Split(csvData, vbCrLf)
    ReDim rows(1 To UBound(csvRows) + 1, 1 To 14)
    
    For i = LBound(csvRows) To UBound(csvRows)
        csvCols = Split(csvRows(i), ",")
        For j = LBound(csvCols) To UBound(csvCols)
            rows(i + 1, j + 1) = Replace(csvCols(j), "'", "")
        Next j
    Next i
    
    ImportCSV = rows
End Function
