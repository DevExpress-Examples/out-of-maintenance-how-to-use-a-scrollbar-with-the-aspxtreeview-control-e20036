Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports DevExpress.Web.ASPxTreeList
Imports System.Data
Imports System.Data.OleDb
Imports DevExpress.Web.ASPxTreeView
Imports System.Web.Configuration

Partial Public Class _Default
	Inherits System.Web.UI.Page
	Private myConnString As String = WebConfigurationManager.ConnectionStrings("NorthwindConnectionString").ConnectionString

	Protected Sub treeView_VirtualModeCreateChildren(ByVal source As Object, ByVal e As DevExpress.Web.ASPxTreeView.TreeViewVirtualModeCreateChildrenEventArgs)
		Dim nodeList As New List(Of TreeViewVirtualNode)()
		Dim nodes As New Dictionary(Of String, String)()
		If String.IsNullOrEmpty(e.NodeName) Then
			nodes = GetParentFromDB("null")
		Else
			nodes = GetParentFromDB(e.NodeName)
		End If
		For Each nodeKeys As KeyValuePair(Of String, String) In nodes
			Dim node As New TreeViewVirtualNode(nodeKeys.Key, nodeKeys.Value)
			If isLeafNode(nodeKeys.Key) Then
				node.IsLeaf = True
			End If
			nodeList.Add(node)
		Next nodeKeys
		e.Children = nodeList
	End Sub

	Private Function GetParentFromDB(ByVal parentID As String) As Dictionary(Of String, String)
		Dim results As New Dictionary(Of String, String)()
		If parentID = "null" Then
			AccessDataSource1.SelectCommand = "SELECT [EmployeeID], [FirstName], [LastName] FROM [Employees] WHERE ([ReportsTo] is null)"
		Else
			AccessDataSource1.SelectCommand = String.Format("SELECT [EmployeeID], [FirstName], [LastName] FROM [Employees] WHERE ([ReportsTo] = {0})", parentID)
		End If
		Dim dv As DataView = CType(AccessDataSource1.Select(DataSourceSelectArguments.Empty), DataView)
		For Each view As DataRowView In dv
			results.Add(view(0).ToString(), view(2) & ", " & view(1))
		Next view
		Return results
	End Function

	Private Function isLeafNode(ByVal nodeID As String) As Boolean
		AccessDataSource1.SelectCommand = String.Format("SELECT [EmployeeID] FROM [Employees] WHERE ([ReportsTo] = {0})", nodeID)
		Dim dv As DataView = CType(AccessDataSource1.Select(DataSourceSelectArguments.Empty), DataView)
		Return (dv.Count = 0)
	End Function
End Class