using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vixen.Sys;
using WeifenLuo.WinFormsUI.Docking;

namespace Vixen3DPreview
{
    public partial class SetupForm_Elements : DockContent
    {
        public SetupForm_Elements()
        {
            InitializeComponent();
        }

        private void SetupForm_Elements_Load(object sender, EventArgs e)
        {
            PopulateElementTree(treeElements);
        }

        // 
        // Add the root nodes to the Display Element tree
        //
        public void PopulateElementTree(TreeView tree)
        {
            tree.Nodes.Clear();
            foreach (ElementNode channel in VixenSystem.Nodes.GetRootNodes())
            {
                AddNodeToElementTree(tree.Nodes, channel);
            }
        }

        // 
        // Add each child Display Element or Display Element Group to the tree
        // 
        private void AddNodeToElementTree(TreeNodeCollection collection, ElementNode channelNode)
        {
            TreeNode addedNode = new TreeNode();
            addedNode.Name = channelNode.Id.ToString();
            addedNode.Text = channelNode.Name;
            addedNode.Tag = channelNode;
            collection.Add(addedNode);

            foreach (ElementNode childNode in channelNode.Children)
            {
                AddNodeToElementTree(addedNode.Nodes, childNode);
            }
        }
    }
}
