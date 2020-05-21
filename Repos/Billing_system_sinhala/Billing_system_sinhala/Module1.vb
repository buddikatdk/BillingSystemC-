Imports System.Data.SqlClient
Module Module1
    Public cn As New SqlConnection
    Public cm As New SqlCommand
    Public dr As SqlDataReader

    Sub Connection()
        Try
            cn = New SqlConnection
            cn.ConnectionString = "Data Source=MSI\SQLEXPRESS;Initial Catalog=Billing_system_Sinhala;Integrated Security=True"
            cn.Open()
            cn.Close()

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        End Try
    End Sub
End Module
