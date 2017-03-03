Imports HtmlAgilityPack
Imports System.IO
Imports System.Linq
Imports Microsoft.SqlServer
Imports System.Data.SqlClient



Public Class _Default
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Dim con As New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("serverconnection").ConnectionString)

        Dim command As String = "Select Industry FROM Industries"
        Dim da As New SqlClient.SqlDataAdapter(command, con)
        Dim ds As New DataSet

        da.Fill(ds, "Industries")





        Dim URL As New HtmlAgilityPack.HtmlDocument
        URL.Load("www.google.com")

        For Each tag As HtmlAgilityPack.HtmlNode In URL.DocumentNode.ChildNodes

            MsgBox(tag.Attributes("class").Value.ToString)

        Next

    End Sub
End Class