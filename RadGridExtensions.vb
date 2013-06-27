'
' Class:  RDCRadGridExtensions
' Author: Dermot O Sullivan
' Date: 27-June-2013
'
' Purpose: Enables RadGrid to remember state of filters, paging and sorting in between postbacks



Imports Microsoft.VisualBasic
Imports Telerik.Web.UI.RadGrid
Imports Telerik.Web.UI
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

Public Module RDCRadGridExtensions

    Public Const kFilterExpression As String = "filter-expression"
    Public Const kFilterFieldValues As String = "filter-field-collection"

    <Extension()>
    Public Sub EnableStateManagement(ByVal grid As Telerik.Web.UI.RadGrid)
        AddHandler grid.PreRender, AddressOf PreRender
        AddHandler grid.NeedDataSource, AddressOf NeedsDataSource

    End Sub

    Friend Sub PreRender(ByVal sender As Object, ByVal e As EventArgs)
        RememberState(sender)
    End Sub

    Friend Sub NeedsDataSource(ByVal sender As Object, ByVal e As EventArgs)
        RestoreState(sender)
    End Sub

    <Extension()>
    Public Sub RememberState(ByVal grid As Telerik.Web.UI.RadGrid)
        If (grid.Page.IsPostBack) Then
            Dim columnFilters As New List(Of KeyValuePair(Of String, String))
            Dim gridSettings As New Dictionary(Of String, Object)
            For Each column As GridColumn In grid.MasterTableView.Columns
                Dim columnFilterState As New KeyValuePair(Of String, String)(column.UniqueName, column.CurrentFilterValue)
                columnFilters.Add(columnFilterState)
            Next

            gridSettings.Add(kFilterExpression, grid.MasterTableView.FilterExpression)
            gridSettings.Add(kFilterFieldValues, columnFilters)
            grid.Page.Session.Add(grid.ID, gridSettings)
        End If
    End Sub

    <Extension()>
    Public Sub RestoreState(ByVal grid As Telerik.Web.UI.RadGrid)
        If (grid.Page.IsPostBack = False) Then
            If (grid.Page.Session(grid.ID) IsNot Nothing) Then
                Dim gridSettings As Dictionary(Of String, Object) = CType(grid.Page.Session(grid.ID), Dictionary(Of String, Object))
                Dim columnFilters As List(Of KeyValuePair(Of String, String)) = CType(gridSettings(kFilterFieldValues), List(Of KeyValuePair(Of String, String)))
                grid.MasterTableView.FilterExpression = gridSettings(kFilterExpression).ToString()

                For Each columnFilterSetting As KeyValuePair(Of String, String) In columnFilters
                    For Each column As GridColumn In grid.MasterTableView.Columns
                        If (column.UniqueName = columnFilterSetting.Key) Then
                            column.CurrentFilterValue = columnFilterSetting.Value
                        End If
                    Next
                Next
            End If
        End If
    End Sub
End Module
