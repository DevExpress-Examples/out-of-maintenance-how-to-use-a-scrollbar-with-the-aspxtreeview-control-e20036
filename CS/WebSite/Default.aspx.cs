using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxTreeList;
using System.Data;
using System.Data.OleDb;
using DevExpress.Web.ASPxTreeView;
using System.Web.Configuration;

public partial class _Default : System.Web.UI.Page
{
    string myConnString = WebConfigurationManager.ConnectionStrings["NorthwindConnectionString"].ConnectionString;

    protected void treeView_VirtualModeCreateChildren(object source, DevExpress.Web.ASPxTreeView.TreeViewVirtualModeCreateChildrenEventArgs e)
    {
        List<TreeViewVirtualNode> nodeList = new List<TreeViewVirtualNode>();
        Dictionary<string, string> nodes = new Dictionary<string, string>();
        if (String.IsNullOrEmpty(e.NodeName))
            nodes = GetParentFromDB("null");
        else
            nodes = GetParentFromDB(e.NodeName);
        foreach (KeyValuePair<string, string> nodeKeys in nodes)
        {
            TreeViewVirtualNode node = new TreeViewVirtualNode(nodeKeys.Key, nodeKeys.Value);
            if (isLeafNode(nodeKeys.Key)) node.IsLeaf = true;
            nodeList.Add(node);
        }
        e.Children = nodeList;
    }

    private Dictionary<string, string> GetParentFromDB(string parentID)
    {
        Dictionary<string, string> results = new Dictionary<string, string>();
        if (parentID == "null")
            AccessDataSource1.SelectCommand = "SELECT [EmployeeID], [FirstName], [LastName] FROM [Employees] WHERE ([ReportsTo] is null)";
        else
            AccessDataSource1.SelectCommand = string.Format("SELECT [EmployeeID], [FirstName], [LastName] FROM [Employees] WHERE ([ReportsTo] = {0})", parentID);
        DataView dv = (DataView)AccessDataSource1.Select(DataSourceSelectArguments.Empty);
        foreach (DataRowView view in dv)
            results.Add(view[0].ToString(), view[2] + ", " + view[1]);
        return results;
    }

    private bool isLeafNode(string nodeID)
    {
        AccessDataSource1.SelectCommand = string.Format("SELECT [EmployeeID] FROM [Employees] WHERE ([ReportsTo] = {0})", nodeID);
        DataView dv = (DataView)AccessDataSource1.Select(DataSourceSelectArguments.Empty);
        return (dv.Count == 0);
    }
}