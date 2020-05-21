Imports System.Data.SqlClient
Public Class Form1
    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        Me.Dispose()
    End Sub

    Private Sub TextBox6_TextChanged(sender As Object, e As EventArgs) Handles textQty.TextChanged
        CalculateTotal()
    End Sub

    Private Sub textQty_KeyPress(sender As Object, e As KeyPressEventArgs) Handles textQty.KeyPress
        Select Case Asc(e.KeyChar)
            Case 48 To 57
            Case 46
            Case 8
            Case Else
                e.Handled = True
        End Select
    End Sub

    Private Sub textUnitPrice_KeyPress(sender As Object, e As KeyPressEventArgs) Handles textUnitPrice.KeyPress
        Select Case Asc(e.KeyChar)
            Case 48 To 57
            Case 46
            Case 8
            Case Else
                e.Handled = True
        End Select
    End Sub

    Sub CalculateTotal()
        Try
            textTotal.Text = Format(((CDbl(textUnitPrice.Text) * CDbl(textQty.Text)) - CDbl(textDiscount.Text)), "#,##0.00")
        Catch ex As Exception
            textTotal.Text = "0.00"
        End Try
    End Sub

    Private Sub textDiscount_KeyPress(sender As Object, e As KeyPressEventArgs) Handles textDiscount.KeyPress
        Select Case Asc(e.KeyChar)
            Case 48 To 57
            Case 46
            Case 8
            Case Else
                e.Handled = True
        End Select
    End Sub

    Private Sub textTotal_KeyPress(sender As Object, e As KeyPressEventArgs) Handles textTotal.KeyPress
        Select Case Asc(e.KeyChar)
            Case 48 To 57
            Case 46
            Case 8
            Case Else
                e.Handled = True
        End Select
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Connection()
    End Sub

    Function GetInvoice() As String
        Try
            Dim sdate As String = Now.ToString("MMddyy")
            cn.Open()
            cm = New SqlCommand("select top 1 invoiceNo from tableBill where invoiceNo like '" & sdate & "%' order by id desc", cn)
            dr = cm.ExecuteReader
            dr.Read()
            If dr.HasRows Then GetInvoice = CLng(dr.Item("invoiceNo").ToString()) + 1 Else GetInvoice = sdate & "1001"
            dr.Close()
            cn.Close()
        Catch ex As Exception
            cn.Close()
            MsgBox(ex.Message, vbCritical)
        End Try
    End Function

    Private Sub btnNewBill_Click(sender As Object, e As EventArgs) Handles btnNewBill.Click
        textInvoiceNo.Text = GetInvoice()
    End Sub

    Private Sub btnAddToList_Click(sender As Object, e As EventArgs) Handles btnAddToList.Click
        Try
            If textBill.Text = String.Empty Or textAdress.Text = String.Empty Or textInvoiceNo.Text = String.Empty Or textInvoiceFor.Text = String.Empty Then
                MsgBox("Required missing field!", vbCritical)
                Return
            End If

            If textDescription.Text = String.Empty Or textQty.Text = String.Empty Or textUnitPrice.Text = String.Empty Or textDiscount.Text = String.Empty Then
                MsgBox("Required missing field!", vbCritical)
                Return
            End If

            cn.Open()
            cm = New SqlCommand("Insert into tableBill_items (invoiceNo,description,quentity,unitPrice,discount)values(@invoiceNo,@description,@quentity,@unitPrice,@discount)", cn)
            With cm.Parameters
                .AddWithValue("@invoiceNo", textInvoiceNo.Text)
                .AddWithValue("@description", textDescription.Text)
                .AddWithValue("@quentity", CDbl(textQty.Text))
                .AddWithValue("@unitPrice", CDbl(textUnitPrice.Text))
                .AddWithValue("@discount", CDbl(textDiscount.Text))
            End With
            cm.ExecuteNonQuery()
            cn.Close()

            LoadRecords()

        Catch ex As Exception
            cn.Close()
            MsgBox(ex.Message, vbCritical)
        End Try

    End Sub

    Private Sub textDiscount_TextChanged(sender As Object, e As EventArgs) Handles textDiscount.TextChanged
        CalculateTotal()
    End Sub

    Private Sub textUnitPrice_TextChanged(sender As Object, e As EventArgs) Handles textUnitPrice.TextChanged
        CalculateTotal()
    End Sub

    Sub LoadRecords()
        Dim _subTotal As Double = 0
        DataGridView1.Rows.Clear()
        Dim i As Integer
        cn.Open()
        cm = New SqlCommand("select * from tableBill_items where invoiceNo like '" & textInvoiceNo.Text & "'", cn)
        dr = cm.ExecuteReader()
        While dr.Read
            i += 1
            _subTotal += CDbl(dr.Item("totalPrice").ToString())
            DataGridView1.Rows.Add(dr.Item("id").ToString(), i, dr.Item("description").ToString(), Format(CDbl(dr.Item("quentity").ToString()), "#,##0.00"), Format(CDbl(dr.Item("unitPrice").ToString()), "#,##0.00"), Format(CDbl(dr.Item("discount").ToString()), "#,##0.00"), Format(CDbl(dr.Item("totalPrice").ToString()), "#,##0.00"), "EDIT", "DELETE")
        End While
        dr.Close()
        cn.Close()

        textSubTotal.Text = Format(_subTotal, "#,##0.00")
        CalculateBill()
    End Sub
    Sub UpdateBill()

    End Sub
    Sub CalculateBill()
        Try
            textSalesTax.Text = Format(CDbl(textSubTotal.Text) * CDbl(textTaxRate.Text), "#,##0.00")
            textTotalAmount.Text = Format(CDbl(textSubTotal.Text) + CDbl(textTaxRate.Text) + CDbl(textOthers.Text), "#,##0.00")
        Catch ex As Exception
            textTotalAmount.Text = "0.00"
        End Try
    End Sub

    Private Sub textTaxRate_KeyPress(sender As Object, e As KeyPressEventArgs) Handles textTaxRate.KeyPress
        If Asc(e.KeyChar) = 13 Then
            UpdateBill()
        End If
    End Sub

    Private Sub textOthers_KeyPress(sender As Object, e As KeyPressEventArgs) Handles textOthers.KeyPress
        If Asc(e.KeyChar) = 13 Then
            UpdateBill()
        End If
    End Sub

    Private Sub textTaxRate_TextChanged(sender As Object, e As EventArgs) Handles textTaxRate.TextChanged
        CalculateBill()
    End Sub

    Private Sub textOthers_TextChanged(sender As Object, e As EventArgs) Handles textOthers.TextChanged
        CalculateBill()
    End Sub
End Class
'Dim found As Boolean = False

'cn.Open()
'cm = New SqlCommand("select * from tableBill where invoiceNo like '" & textInvoiceNo.Text & "'", cn)
'dr = cm.ExecuteReader()
'If dr.HasRows Then found = True Else found = False
'            dr.Close()
'cn.Close()

'If found = True Then
'cn.Open()
'cm = New SqlCommand("update tableBill set invoiceDate = @invoiceDate, billTo = @billTo, adderss = @address, invoiceFor = @invoiceFor, subTotal = @subTotal, taxRate = @taxRate, salesTax = @salesTax, others = @others where invoiceNo = @invoiceNo", cn)
'With cm.Parameters
'.AddWithValue("@invoiceDate", textInvoiceNo.Text)
'.AddWithValue("@billTo", textDescription.Text)
'.AddWithValue("@adderss", textQty.Text)
'.AddWithValue("@invoiceFor", textUnitPrice.Text)
'.AddWithValue("@subTotal", CDbl(textDiscount.Text))
'.AddWithValue("@taxrate", CDbl(textTaxRate.Text))
'.AddWithValue("@salesTax", CDbl(textSalesTax.Text))
'.AddWithValue("@others", CDbl(textOthers.Text))
'.AddWithValue("@invoiceNo", textInvoiceNo.Text)
'End With
'cm.ExecuteNonQuery()
'cn.Close()

'cn.Open()
'cm = New SqlCommand("Insert into tableBill (invoiceNo, invoiceDate, billTo, address, invoiceFor, subTotal, taxRate, salesTax, others)values(@invoiceNo, @invoiceDate, @billTo, @address, @invoiceFor, @subTotal, @taxRate, @salesTax, @others)", cn)
'With cm.Parameters
'.AddWithValue("@invoiceNo", textInvoiceNo.Text)
'.AddWithValue("@invoiceDate", textInvoiceNo.Text)
'.AddWithValue("@billTo", textDescription.Text)
'.AddWithValue("@adderss", textQty.Text)
'.AddWithValue("@invoiceFor", textUnitPrice.Text)
'.AddWithValue("@subTotal", CDbl(textDiscount.Text))
'.AddWithValue("@taxrate", CDbl(textTaxRate.Text))
'.AddWithValue("@salesTax", CDbl(textSalesTax.Text))
'.AddWithValue("@others", CDbl(textOthers.Text))
'End With
'cm.ExecuteNonQuery()
'cn.Close()
'End If