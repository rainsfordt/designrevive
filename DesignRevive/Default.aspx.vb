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
        Dim dt_Source As DataTable = dbfunction.SELECTSTATEMENT("SourceName, URL", "BusinessSources", "WHERE SourceName = 'Yell.com'")
        Dim dt_Industry As DataTable = dbfunction.SELECTSTATEMENT("TOP 10 IndustryName", "FullIndustryList", "")
        Dim dt_Towns As DataTable = dbfunction.SELECTSTATEMENT("TOP 1 TownName", "Towns", "")
        Dim SourceName As String = dt_Source.Rows(0).Item(0).ToString()
        Dim URL As String = dt_Source.Rows(0).Item(1).ToString()


        For Each IndustryRow As DataRow In dt_Industry.Rows()

            Dim Industry_URL = URL.Replace("%industry%", IndustryRow.Item(0).ToString)

            For Each TownRow As DataRow In dt_Towns.Rows()

                Dim URLFormatted = Industry_URL.Replace("%area%", TownRow.Item(0).ToString)

                'MsgBox(URL)
                'TryAgain:
                'If ThreadCount < 10 Then

                Dim Source As String = SourceName & "#" & URLFormatted ' Merge the two strings into 1 to pass into thread
                    Dim Thread As New System.Threading.Thread(AddressOf Scraper)

                    Thread.IsBackground = True

                    Thread.Start(Source) 'You can only pass 1 value into a thread so I have combined the SourceName & URL as Source to pass through the thread

                    ThreadCount = ThreadCount + 1

                'Else

                'GoTo TryAgain

                'End If



            Next

        Next

    End Sub

    'Now we want to scrape the data based on the url
    Private Sub Scraper(ByVal Source As String)

        Dim Parts As String() = Source.Split(New Char() {"#"c}) 'Seperate out the string now that it has been passed through
        Dim SourceName As String = Parts(0).ToString
        Dim URL As String = Parts(1).ToString

        Dim Web As New HtmlWeb
        Dim BusinessSource As New HtmlAgilityPack.HtmlDocument
        Dim db As New DatabaseActions

        BusinessSource = Web.Load(URL)

        Dim BusinessCollection = BusinessSource.DocumentNode.SelectNodes("//div[contains(@class,'js-LocalBusiness')]")

        For Each Node As HtmlAgilityPack.HtmlNode In BusinessCollection

            Dim NodeBusinessName As String = Node.SelectSingleNode(".//h2[@itemprop='name']").InnerHtml.ToString

            Dim NodeBusinessWebsite As String = Node.SelectSingleNode(".//a[@itemprop='url']").Attributes("href").Value.ToString

            Dim NodeBusinessTelephone As String = ""

            'Haven't Found Emails to scrape as of yet
            'If IsNothing(Node.SelectSingleNode("//strong[@itemprop='telephone']")) Then

            '    Dim NodeBusinessEmail As String = "NULL"

            'Else

            '    Dim NodeBusinessEmail As HtmlNode = 

            'End If

            If IsNothing(Node.SelectSingleNode(".//strong[@itemprop='telephone']")) Then

                NodeBusinessTelephone = "NULL"

            Else

                NodeBusinessTelephone = Node.SelectSingleNode(".//strong[@itemprop='telephone']").InnerHtml.ToString()

            End If

            db.INSERT("BusinessInformation", "BusinessName, Website, Email, Telephone, SourceName", "'" & (NodeBusinessName & "', '" & NodeBusinessWebsite & "', Null, '" & NodeBusinessTelephone & "', '" & SourceName & "'"))

        Next

    End Sub

End Class