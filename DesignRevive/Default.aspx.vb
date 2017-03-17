Imports HtmlAgilityPack
Imports System.IO
Imports System.Linq
Imports Microsoft.SqlServer
Imports System.Data.SqlClient
Imports DesignRevive.DatabaseActions

Public Module GlobalVariables
    Public ThreadCount As Int16 = 0
    Public dbfunction As New DatabaseActions
End Module

Public Class _Default
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    End Sub

    Protected Sub ScrapeYell_Click() Handles ScrapeYell.Click

        'Dim dbfunction As New DatabaseActions
        Dim dt_Source As DataTable = dbfunction.SELECTSTATEMENT("SourceName, URL", "BusinessSources", "WHERE SourceName = 'Yell.com'")
        Dim dt_Industry As DataTable = dbfunction.SELECTSTATEMENT("IndustryName", "FullIndustryList", "WHERE IndustryID > 40")
        Dim dt_Towns As DataTable = dbfunction.SELECTSTATEMENT("TOP 1 TownName", "Towns", "")
        Dim SourceName As String = dt_Source.Rows(0).Item(0).ToString()
        Dim URL As String = dt_Source.Rows(0).Item(1).ToString()


        For Each IndustryRow As DataRow In dt_Industry.Rows()

            Dim Industry_URL = URL.Replace("%industry%", IndustryRow.Item(0).ToString)

            For Each TownRow As DataRow In dt_Towns.Rows()

                Dim URLFormatted = Industry_URL.Replace("%area%", TownRow.Item(0).ToString)

                'MsgBox(URL)
TryAgain:
                If ThreadCount < 1 Then

                    Dim Source As String = SourceName & "#" & URLFormatted ' Merge the two strings into 1 to pass into thread
                    Dim Thread As New System.Threading.Thread(AddressOf Scraper)

                    Thread.IsBackground = True

                    Thread.Start(Source) 'You can only pass 1 value into a thread so I have combined the SourceName & URL as Source to pass through the thread

                    ThreadCount = ThreadCount + 1

                Else

                    GoTo TryAgain

                End If



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

ThreadReset:

        Threading.Thread.Sleep(10000)

        BusinessSource = Web.Load(URL)

        Dim BusinessCollection = BusinessSource.DocumentNode.SelectNodes("//div[contains(@class,'js-LocalBusiness')]")

        If Not IsNothing(BusinessCollection) Then

            For Each Node As HtmlAgilityPack.HtmlNode In BusinessCollection

                'Get Business Name

                Dim NodeBusinessName As String = "" 'Node.SelectSingleNode(".//h2[@itemprop='name']").InnerHtml.ToString

                If (Node.SelectSingleNode(".//h2[@itemprop='name']").InnerHtml.ToString).Contains("'") Then

                    Dim ReplaceName As String = "''"

                    NodeBusinessName = Node.SelectSingleNode(".//h2[@itemprop='name']").InnerHtml.ToString.Replace("'", ReplaceName)

                Else

                    NodeBusinessName = Node.SelectSingleNode(".//h2[@itemprop='name']").InnerHtml.ToString

                End If

                'Get Website

                Dim URLNode As HtmlNode = Node.SelectSingleNode(".//a[@itemprop='url']")
                Dim NodeBusinessWebsite As String = ""
                If Not IsNothing(URLNode) Then

                    NodeBusinessWebsite = Node.SelectSingleNode(".//a[@itemprop='url']").Attributes("href").Value.ToString()

                Else

                    NodeBusinessWebsite = "NULL"

                End If



                'If IsNothing(NodeBusinessWebsite) Then

                '    NodeBusinessWebsite = "NULL"

                'Else

                '    NodeBusinessWebsite = Node.SelectSingleNode(".//a[@itemprop='url']").Attributes("href").Value.ToString()

                'End If

                ' Get Telephone

                Dim NodeBusinessTelephone As String = ""

                If IsNothing(Node.SelectSingleNode(".//strong[@itemprop='telephone']")) Then

                    NodeBusinessTelephone = "NULL"

                Else

                    NodeBusinessTelephone = Node.SelectSingleNode(".//strong[@itemprop='telephone']").InnerHtml.ToString()

                End If

                'Haven't Found Emails to scrape as of yet
                'If IsNothing(Node.SelectSingleNode("//strong[@itemprop='telephone']")) Then

                '    Dim NodeBusinessEmail As String = "NULL"

                'Else

                '    Dim NodeBusinessEmail As HtmlNode = 

                'End If

                db.INSERT("BusinessInformation", "BusinessName, Website, Email, Telephone, SourceName", "'" & (NodeBusinessName & "', '" & NodeBusinessWebsite & "', Null, '" & NodeBusinessTelephone & "', '" & SourceName & "'"))

            Next

            ThreadCount = ThreadCount - 1

        Else

            Threading.Thread.Sleep(5000)

            GoTo ThreadReset

        End If

    End Sub

    'Now that we have the URL's we want to analyze them

    Private Sub Analyzer(ByVal Replace As String) Handles GA.Click

        Dim dt_AnalyzerSource As DataTable = dbfunction.SELECTSTATEMENT("SourceName, URL", "AnalyzerSources", "WHERE SourceName = 'Google Analyzer'")
        Dim dt_Website As DataTable = dbfunction.SELECTSTATEMENT("TOP 1 Website", "BusinessInformation", "WHERE DateChecked = NULL OR DateChecked <= FORMAT(DATEADD(MM, -1, GETDATE()), 'yyyy-MM-dd') + ' 00:00:00'")
        Dim dr_Website_URL_Unformatted As String = dt_Website.Rows(0).Item(0).ToString()
        Dim dr_AnalyzerSource_URL As String = dt_AnalyzerSource.Rows(0).Item(1).ToString()

        For Each WebsiteRow As DataRow In dt_Website.Rows()



            Dim dr_AnalyzerSource_URL_Formatted = dr_AnalyzerSource_URL.Replace("%sitename%", "hi")

            For Each TownRow As DataRow In dt_Towns.Rows()

                Dim URLFormatted = Industry_URL.Replace("%area%", TownRow.Item(0).ToString)




            Next

        Next

    End Sub
    Public Sub WebsiteFormatted()
        Dim Replace As New ArrayList
        Replace.Add("Replace")

    End Sub

End Class


