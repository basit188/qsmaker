Imports system.Data.OleDb
Imports Excel = Microsoft.Office.Interop.Excel
Imports System.Text.RegularExpressions


Public Class frmMain
    Dim con As OleDbConnection
    Dim cmd As OleDbCommand
    Dim dadp As OleDbDataAdapter
    Dim dtable As DataTable
    Dim dview As DataView
    Dim total As Double
    Dim filename As String = ""


    Private Sub frmMain_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        con.Close()
    End Sub

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'MsgBox(Application.StartupPath)
        con = New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=qty.accdb;")
        con.Open()
        dadp = New OleDbDataAdapter("SELECT * FROM tbQty ORDER BY Item ASC", con)

        dtable = New DataTable()
        dadp.Fill(dtable)
        dview = New DataView(dtable)
        dgSearch.DataSource = dview
        dgSearch.SelectionMode = DataGridViewSelectionMode.FullRowSelect

        dgSearch.Columns.Item(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        dgSearch.Columns.Item(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        dgSearch.Columns.Item(2).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        dgSearch.Columns.Item(3).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells

        dgQs.Columns.Item(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        dgQs.Columns.Item(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        dgQs.Columns.Item(2).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        dgQs.Columns.Item(3).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        dgQs.Columns.Item(4).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        dgQs.Columns.Item(5).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
    End Sub

    Private Sub txtSearch_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtSearch.TextChanged
        refreshDb()
        dview.RowFilter = "Item like '%" & txtSearch.Text & "%'"
    End Sub

    Private Sub dgSearch_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgSearch.CellClick
        Dim sno, item, unit, rate As String
        Dim str(5) As String
        If (e.RowIndex > -1) Then
            item = dgSearch.Rows(e.RowIndex).Cells(1).Value
            unit = dgSearch.Rows(e.RowIndex).Cells(2).Value
            rate = dgSearch.Rows(e.RowIndex).Cells(3).Value
            sno = dgQs.Rows.Count
            str(0) = sno
            str(1) = item
            str(2) = "0.0"
            str(3) = unit
            str(4) = rate
            str(5) = "0.0"
            dgQs.Rows.Add(str)
        End If
    End Sub

    Private Sub ExitToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub AddItemToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddItemToolStripMenuItem.Click
        Dim fai As New frmAddItem()
        fai.Show()
    End Sub

    Private Sub RefreshToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshToolStripMenuItem.Click
        refreshDb()
    End Sub

    Private Sub dgQs_CellValueChanged(ByVal s As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgQs.CellValueChanged
        'MsgBox(e.RowIndex + vbNewLine + e.ColumnIndex)
        'MsgBox("here")
        Dim total As Double = 0
        If (e.RowIndex <> -1) Then
            Dim i As Integer
            For i = 0 To dgQs.RowCount - 1
                dgQs.Item(5, i).Value = dgQs.Item(4, i).Value * dgQs.Item(2, i).Value
            Next
            For i = 0 To dgQs.RowCount - 1
                total = total + dgQs.Item(5, i).Value
            Next
            txtTotal.Text = total
        End If
        Me.total = total
    End Sub

    Private Sub dgSearch_CellMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles dgSearch.CellMouseClick
        If (e.Button = Windows.Forms.MouseButtons.Right AndAlso e.RowIndex > -1) Then
            cmsSearch.Show(New System.Drawing.Point(Windows.Forms.Cursor.Position))
            'rIndex = e.RowIndex
        End If
    End Sub
    Private Function validateData() As Boolean
        Dim itemName As String = txtItemName.Text        
        filename = txtItemName.Text
        Dim nfname As String = ""
        Dim rexp As New Regex("[~#%*{}:<>?/+|\\""]")
        filename = rexp.Replace(filename, "")
        If (filename.Trim() = "") Then
            MsgBox("filename incorrect or empty")
            validateData = False
        End If
        validateData = True
    End Function
    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If (validateData()) Then
            Dim app As New Excel.Application
            Dim wbook As Excel.Workbook
            Dim wsheet As Excel.Worksheet            
            If (txtItemName.Text = "") Then
                filename = "temp"
            End If
            Dim location As String = Application.StartupPath & "\" & filename
            wbook = app.Workbooks.Add()
            wsheet = wbook.ActiveSheet


            wsheet.Cells(1, 1) = txtItemName.Text

            wsheet.Cells.Range(wsheet.Cells(1, 1), wsheet.Cells(1, 6)).Merge()

            For j As Integer = 0 To dgQs.ColumnCount - 1
                wsheet.Cells(2, j + 1) = dgQs.Columns(j).Name
            Next
            Dim i As Integer
            For i = 0 To dgQs.RowCount - 2
                For j As Integer = 0 To dgQs.ColumnCount - 1
                    wsheet.Cells((i + 3), (j + 1)) = dgQs.Item(j, i).Value
                Next
            Next

            wsheet.Cells(i + 3, 5) = "TOTAL"
            wsheet.Cells(i + 3, 6) = total
            app.Visible = True
            wbook.SaveAs(location)

            wbook.Close()
            app.Quit()

            app = Nothing
        End If
    End Sub

    Private Sub DeleteToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteToolStripMenuItem.Click
        deleteItem()
    End Sub

    Private Sub refreshDb()
        dtable.Clear()
        dadp.Fill(dtable)
    End Sub

    Private Sub EditToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditToolStripMenuItem.Click
        editItem()
    End Sub

    Private Sub dgQs_CellMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles dgQs.CellMouseClick
        If (e.Button = Windows.Forms.MouseButtons.Right AndAlso e.RowIndex > -1) Then
            dgQs.Rows.Remove(dgQs.Rows(e.RowIndex))
        End If
    End Sub

    Private Sub dgSearch_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles dgSearch.KeyDown        
        If (e.KeyValue = 113) Then
            'MsgBox("f2 pressed" & vbNewLine & dgSearch.CurrentCell().Value)
            editItem()
        ElseIf (e.KeyValue = Keys.Delete) Then
            deleteItem()
        End If
    End Sub

    Private Sub editItem()
        Dim rIndex As Integer
        Dim fm As frmAddItem
        'Dim id As Integer = Integer.Parse(dgSearch.CurrentCell().Value)
        rIndex = dgSearch.CurrentRow.Index
        Dim id As Integer = dgSearch.Item(0, rIndex).Value
        Dim item As String = dgSearch.Item(1, rIndex).Value
        Dim unit As String = dgSearch.Item(2, rIndex).Value
        Dim rate As String = dgSearch.Item(3, rIndex).Value
        fm = New frmAddItem(id, item, unit, rate)
        fm.ShowDialog()
        refreshDb()
    End Sub

    Private Sub deleteItem()
        Dim rIndex As Integer
        rIndex = dgSearch.CurrentRow.Index
        If (MsgBox("Are you sure you wan to delete" & vbNewLine & dgSearch.Item(1, rIndex).Value & " ?", MsgBoxStyle.YesNo, "Delete Item")) Then
            Dim delQuery As String = "DELETE FROM tbQty WHERE ID = " & dgSearch.Item(0, rIndex).Value
            cmd = New OleDbCommand(delQuery, con)
            cmd.CommandText = delQuery
            cmd.ExecuteNonQuery()
            refreshDb()
        End If
    End Sub

End Class