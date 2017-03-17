﻿Imports System.Web
Imports System.Net
Imports System.Web.Script.Serialization
Imports System.IO

Imports System.Data.SqlClient

Public Module GlobalVars

    Public con As New SqlConnection(ConfigurationManager.ConnectionStrings("DesignReviveDB").ConnectionString)
    Public command As New SqlCommand

End Module

Public Class DatabaseActions
    Implements IHttpModule

    Private WithEvents _context As HttpApplication

    ''' <summary>
    '''  You will need to configure this module in the Web.config file of your
    '''  web and register it with IIS before being able to use it. For more information
    '''  see the following link: http://go.microsoft.com/?linkid=8101007
    ''' </summary>
#Region "IHttpModule Members"

    Public Sub Dispose() Implements IHttpModule.Dispose

        ' Clean-up code here

    End Sub

    Public Sub Init(ByVal context As HttpApplication) Implements IHttpModule.Init
        _context = context
    End Sub

#End Region

    Public Sub OnLogRequest(ByVal source As Object, ByVal e As EventArgs) Handles _context.LogRequest

        ' Handles the LogRequest event to provide a custom logging 
        ' implementation for it

    End Sub

    Public Sub UPDATESession(ByVal table As String, value As String)

        Dim result As String = ""

        Try
            command.CommandType = CommandType.Text
            command.CommandText = "UPDATE [" & table & "] SET [SessionToken] = '" & value & "' WHERE [SessionToken] = '-1'"
            command.Connection = con
            con.Open()
            command.ExecuteNonQuery()
            con.Close()
            result = "Success:" & command.CommandText.ToString

        Catch ex As Exception
            result = ex.Message.ToString
        End Try

    End Sub 'For PuntersEdge ONLY - deals with session token for API access
    Public Sub INSERT(ByVal table As String, ByVal columns As String, values As String)

        Try
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("DesignReviveDB").ConnectionString)

                Dim sqlcommand As New SqlCommand

                sqlcommand.CommandText = "INSERT INTO " & table & "(" & columns & ")" & " VALUES (" & values & ")"
                sqlcommand.Connection = connection

                connection.Open()
                sqlcommand.ExecuteNonQuery()
                connection.Close()

            End Using
            '            command.CommandText = "INSERT INTO " & table & "(" & columns & ")" & " VALUES (" & values & ")"
            '            command.Connection = con
            'TryAgain:
            '            If con.State = ConnectionState.Closed Then
            '                con.Open()
            '                command.ExecuteNonQuery()
            '                con.Close()
            '            Else

            '                GoTo TryAgain

            '            End If

        Catch ex As Odbc.OdbcException
            MsgBox(ex.ToString)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try


    End Sub 'General INSERT statement. Accepts table name, columns in string (column1, column2 etc), and values in string ('varchar', int etc)
    Public Function SELECTSTATEMENT(ByVal ColumnsToSelect As String, ByVal table As String, ByVal whereClause As String)


        Dim strSql As String = "SELECT " & ColumnsToSelect & " FROM " & table & " " & whereClause
        Dim Resultset As New DataTable
        'Using con As New SqlConnection
        con.Open()
            Using dad As New SqlDataAdapter(strSql, con)
                dad.Fill(Resultset)
            End Using
            con.Close()
        'End Using


        Return Resultset
    End Function 'General Select statement. Accepts table name, columns or *, and a where clause if needed (if not, pass in "")
    Public Function EXECSPROC(ByVal SPROCNAME As String, variables As String)
        Dim Result As String = ""

        command.CommandText = "EXECUTE " & SPROCNAME & " " & variables
        command.Connection = con

        Try

            con.Open()
            Result = command.ExecuteScalar
            con.Close()

            Result = "Database Cleansed"

        Catch ex As Exception

            Result = ex.ToString

        End Try

        Return Result

    End Function

End Class