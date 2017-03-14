Imports HtmlAgilityPack
Imports System.IO
Imports System.Linq
Imports Microsoft.SqlServer
Imports System.Data.SqlClient
Imports DesignRevive.DatabaseActions

Public Module GlobalVariables
    Public ThreadCount As Int16 = 0
End Module

Public Class _Default
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        'Dim con As New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("serverconnection").ConnectionString)

        'Dim command As String = "Select Industry FROM Industries"
        'Dim da As New SqlClient.SqlDataAdapter(command, con)
        'Dim ds As New DataSet

        'da.Fill(ds, "Industries")





        'Dim URL As New HtmlAgilityPack.HtmlDocument
        'URL.Load("www.google.com")

        'For Each tag As HtmlAgilityPack.HtmlNode In URL.DocumentNode.ChildNodes

        '    MsgBox(tag.Attributes("class").Value.ToString)

        'Next

    End Sub

    Protected Sub ScrapeYell_Click() Handles ScrapeYell.Click
        Dim dbfunction As New DatabaseActions
        Dim dt_SourceURL As DataTable = dbfunction.SELECTSTATEMENT("URL", "BusinessSources", "WHERE SourceName = 'Yell.com'")
        Dim dt_Industry As DataTable = dbfunction.SELECTSTATEMENT("TOP 10 IndustryName", "FullIndustryList", "")
        Dim dt_Towns As DataTable = dbfunction.SELECTSTATEMENT("TOP 2 TownName", "Towns", "")
        Dim URL As String = dt_SourceURL.Rows(0).Item(0).ToString()

        For Each IndustryRow As DataRow In dt_Industry.Rows()

            URL = URL.Replace("%industry%", IndustryRow.Item(0).ToString)

            For Each TownRow As DataRow In dt_Towns.Rows()

                URL = URL.Replace("%area%", TownRow.Item(0).ToString)

                'MsgBox(URL)
TryAgain:
                If ThreadCount < 10 Then

                    Dim Thread As New System.Threading.Thread(AddressOf Scraper)

                    Thread.IsBackground = True

                    Thread.Start(URL)

                    ThreadCount = ThreadCount + 1

                Else

                    GoTo TryAgain

                End If

                URL = dt_SourceURL.Rows(0).Item(0).ToString()

            Next

        Next

    End Sub

    'Now we want to scrape the data based on the url
    Private Sub Scraper(ByVal URL As String)

        Dim Web As New HtmlWeb
        Dim BusinessSource As New HtmlAgilityPack.HtmlDocument
        Dim db As New DatabaseActions

        BusinessSource = Web.Load(URL)

        Dim BusinessCollection As HtmlNodeCollection = BusinessSource.DocumentNode.SelectNodes("//div[contains(@class,'js-LocalBusiness')]")

        For Each Node As HtmlNode In BusinessCollection.Nodes

            Dim NodeBusinessName As HtmlNode = Node.SelectSingleNode("//h2[@itemprop='name']")

            Dim NodeBusinessWebsite As HtmlNode = Node.SelectSingleNode("//a[@itemprop='url']")

            'Haven't Found Emails to scrape as of yet
            'If IsNothing(Node.SelectSingleNode("//strong[@itemprop='telephone']")) Then

            '    Dim NodeBusinessEmail As String = "NULL"

            'Else

            '    Dim NodeBusinessEmail As HtmlNode = 

            'End If

            If IsNothing(Node.SelectSingleNode("//strong[@itemprop='telephone']")) Then

                Dim NodeBuinessTelephone As String = "NULL"

            Else

                Dim NodeBusinessTelephone As HtmlNode = Node.SelectSingleNode("//strong[@itemprop='telephone']")

            End If

            db.INSERT("BusinessInformation", "BusinessName, Website, Email, Telephone", (NodeBusinessName.InnerHtml.ToString & ", " & NodeBusinessWebsite.InnerHtml.ToString & ", Null, " & NodeBusinessTelephone.InnerHtml.ToString))

            ThreadCount = ThreadCount - 1

        Next

    End Sub

End Class