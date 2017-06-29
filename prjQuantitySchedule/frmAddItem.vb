Imports system.Data.OleDb
Public Class frmAddItem
    Dim con As OleDbConnection
    Dim cmd As OleDbCommand
    Dim id As Integer = -1
    Dim item, unit, rate As String
    Public Sub New()
        InitializeComponent()
    End Sub
    Public Sub New(ByVal i As Integer, ByVal itm As String, ByVal unt As String, ByVal rte As String)
        InitializeComponent()
        id = i
        item = itm
        unit = unt
        rate = rte
    End Sub

    Private Sub txtItem_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtItem.TextChanged
        txtItem.Text = txtItem.Text.Replace(vbNewLine, " ")
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        txtItem.Focus()
        If (id > 0) Then
            'MsgBox(id)
            Me.Text = "Update"
            txtItem.Text = item
            txtUnit.Text = unit
            txtRate.Text = rate
            btnSaveNew.Visible = False
            btnSaveClose.Text = "Update"
        Else
            Me.Text = "Add New"
        End If
    End Sub

    Private Sub btnSaveNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveNew.Click
        SaveItem()
        txtItem.Clear()
        txtRate.Clear()
        txtItem.Focus()
    End Sub

    Private Sub SaveItem()
        con = New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=qty.accdb;")
        cmd = New OleDbCommand()
        cmd.Connection = con
        Dim item, rate, unit As String
        item = txtItem.Text
        rate = txtRate.Text
        unit = txtUnit.Text
        cmd.CommandText = "INSERT INTO tbQty(Item,Unit,Rate) VALUES ('" & item & "','" & unit & "'," & rate & ")"
        con.Open()
        cmd.ExecuteNonQuery()
        con.Close()
    End Sub
    Private Sub UpdateItem()
        con = New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=qty.accdb;")
        cmd = New OleDbCommand()
        cmd.Connection = con
        Dim item, rate, unit As String
        item = txtItem.Text
        rate = txtRate.Text
        unit = txtUnit.Text
        cmd.CommandText = "UPDATE tbQty SET Item = '" & item & "', Unit = '" & unit & "', Rate = " & rate & " WHERE ID=" & id
        con.Open()
        cmd.ExecuteNonQuery()
        con.Close()
    End Sub
    Private Sub btnSaveClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveClose.Click
        If (id > 0) Then
            UpdateItem()
        Else
            SaveItem()
        End If        
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub
End Class
