using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.Diagnostics;
namespace AppMenuSharper
{
    public partial class Form1 : Form
    {
        int imageIndex = 0;
        int lastItemIndex = -2;

        public ListViewItem itemBeingDragged { get; private set; }
        public ListViewItem lastItem { get; private set; }

        public Form1()
        {
            InitializeComponent();
            this.listView1.ListViewItemSorter = new ListViewIndexComparer();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.loadItems();
            this.AllowDrop = true;
            this.trayMenu_fill();
        }

        private void trayMenu_fill()
        {
            ContextMenuStrip trayMenu = new ContextMenuStrip();
            //ToolStripItemCollection trayMenuItems = new ToolStripItemCollection(trayMenu);
            trayMenu.Items.Add("Show", null, this.trayItemShowClick);
            trayMenu.Items.Add("Exit", null, this.trayItemExitClick);

            this.notifyIcon1.ContextMenuStrip = trayMenu;

            this.notifyIcon1.Visible = true;

            System.Collections.ArrayList listItems = new System.Collections.ArrayList();
            foreach( ListViewItem listItem in this.listView1.Items )
            {
                Image image = this.imageList1.Images[listItem.ImageIndex];
                trayMenu.Items.Add(listItem.Text,image,this.trayLauncherItemClick);
            }

        }

        private void trayLauncherItemClick(object sender, EventArgs e)
        {
            Console.WriteLine("trayItemClick:" + sender.GetType());
            //Process.Start(sender.Text);
            if (sender.GetType().FullName == "System.Windows.Forms.ToolStripMenuItem")
            {
                System.Windows.Forms.ToolStripMenuItem item = (System.Windows.Forms.ToolStripMenuItem)sender;
                Process.Start(item.Text);
            }

        }

        private void trayItemExitClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void trayItemShowClick(object sender, EventArgs e)
        {
            this.Activate();
            Console.WriteLine("Show Click!");
        }

        private void Form1_DragLeave(object sender, DragEventArgs e)
        {
            Console.WriteLine("Form1_DragLeave");
            Console.WriteLine(e.Data);
            
        }

        private void loadItems()
        {

            // LauncherItem[] items = JsonSerializer.Deserialize<LauncherItem[]>(JsonDocument.Parse(File.ReadAllText("launcher.json")));
            Dictionary<String, LauncherItem> pkv = JsonSerializer.Deserialize<Dictionary<String,LauncherItem>>(JsonDocument.Parse(File.ReadAllText("launcher.json")));
            Console.WriteLine(pkv);
            foreach (LauncherItem item in pkv.Values)
            {
                this.listView1.Items.Add(item.exe_path);
            }
            //int i = 0;
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.LargeImageList = this.imageList2;
            
            foreach (System.Windows.Forms.ListViewItem item in this.listView1.Items)
            {
                try
                {
                    // item.Text holds the exe path
                    this.imageList1.Images.Add(Icon.ExtractAssociatedIcon(item.Text));
                    this.imageList2.Images.Add(Icon.ExtractAssociatedIcon(item.Text));
                    item.ImageIndex = this.imageIndex;
                    this.imageIndex++;
                }
                catch (Exception e)
                {
                    continue;
                }
                finally
                {
                }
             }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            Console.WriteLine("Form1_DragEnter");
            Console.WriteLine(e.Data.GetFormats());
            //this.itemBeingDragged = (object) e.Data.GetData("object");
            Console.WriteLine(this.itemBeingDragged);
            e.Effect = DragDropEffects.Copy;
        }



        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            Console.WriteLine("Form1_DragDrop");
            //this.DoDragDrop(e.Data, DragDropEffects.Move);
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                foreach (string fileLoc in filePaths)
                {
                    // Code to read the contents of the text file
                    if (File.Exists(fileLoc))
                    {
                        ListViewItem newItem = this.listView1.Items.Add(fileLoc);
                        this.imageList1.Images.Add(Icon.ExtractAssociatedIcon(fileLoc));
                        this.imageList2.Images.Add(Icon.ExtractAssociatedIcon(fileLoc));
                        newItem.ImageIndex = this.imageIndex++;
                    }

                }
            }
        }

        private void Form1_FormClosing(Object sender, FormClosingEventArgs evt)
        {
            this.saveItems();
        }


        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveItems();
        }

        private void saveItems()
        {
            System.Collections.ArrayList items = new System.Collections.ArrayList();
            Dictionary<string, LauncherItem> map = new Dictionary<string, LauncherItem>();
            foreach (System.Windows.Forms.ListViewItem item in this.listView1.Items ) {
                //items.Add(item.Text);
                string textvalue = item.Text;
                LauncherItem launcherItem = new LauncherItem(textvalue);
                items.Add(launcherItem);
                map.Add(launcherItem.title,launcherItem);
            }
            System.Text.Json.JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            String jsonStr = System.Text.Json.JsonSerializer.Serialize(items.ToArray(),options);
            String mapStr = System.Text.Json.JsonSerializer.Serialize(map,options);
            Console.WriteLine(mapStr);
            File.WriteAllText("launcher.json",mapStr);
            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveItems();
            System.Environment.Exit(0);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            Console.WriteLine(sender + ">" + e);

            Console.WriteLine(this.listView1.FocusedItem.Text);
            Process.Start(this.listView1.FocusedItem.Text);
        }

        private void listView1_DragLeave(object sender, EventArgs e)
        {
            
            Console.WriteLine("listView1_DragLeave");
            if (this.itemBeingDragged != null)
            {
 
                
                Console.WriteLine(e.GetType());
                this.listView1.Items.Remove(this.itemBeingDragged);
                this.listView1.Refresh();
                this.itemBeingDragged = null;
            }

            this.listView1.InsertionMark.Index = -1;
        }

        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            Console.WriteLine("ItemDrag!");
            Console.WriteLine(e.Item);
            
            this.itemBeingDragged = (ListViewItem)e.Item;
            //this.lastItemIndex = this.itemBeingDragged.Index;
            //this.listView1.InsertionMark.Index = this.lastItemIndex;
            this.listView1.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void iconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listView1.View = View.LargeIcon;
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listView1.View = View.List;
        }

        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listView1.View = View.Details;
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void tilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listView1.View = View.Tile;
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_ItemMouseHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            Console.WriteLine("listView_ItemHover");

/*            if (this.isDragging)
            {
                this.lastItem = e.Item;

                this.lastItemIndex = e.Item.Index;
                //this.listView1.InsertionMark.Index = this.lastItemIndex;
                Console.WriteLine("lastIndex: " + this.lastItemIndex);
            }*/
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
/*            if (this.itemBeingDragged != null && this.itemBeingDragged.GetType().Name == "ListViewItem")
            {
                this.listView1.Items.Remove(this.itemBeingDragged);
                this.listView1.Items.Insert(this.lastItemIndex, (ListViewItem)this.itemBeingDragged);
                this.isDragging = false;
                this.itemBeingDragged = null;
                this.listView1.Refresh();
                //this.listView1.InsertionMark.Index = -1;
            }
*/
            Console.WriteLine("listView1_MouseUp");
            Console.WriteLine(e.Location);
            
        }

        private void Form1_MouseLeave(object sender, EventArgs e)
        {
            Console.WriteLine("Form1_MouseLeave");
            Console.WriteLine(e.GetType());
        }

        private void listView1_MouseLeave(object sender, EventArgs e)
        {
            Console.WriteLine("listView1_MouseLeave");
        }

        private void listView1_DragOver(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the mouse pointer.
            Point targetPoint =
                this.listView1.PointToClient(new Point(e.X, e.Y));

            // Retrieve the index of the item closest to the mouse pointer.
            int targetIndex = this.listView1.InsertionMark.NearestIndex(targetPoint);

            // Confirm that the mouse pointer is not over the dragged item.
            if (targetIndex > -1)
            {
                // Determine whether the mouse pointer is to the left or
                // the right of the midpoint of the closest item and set
                // the InsertionMark.AppearsAfterItem property accordingly.
                Rectangle itemBounds = this.listView1.GetItemRect(targetIndex);
                if (targetPoint.X > itemBounds.Left + (itemBounds.Width / 2))
                {
                    this.listView1.InsertionMark.AppearsAfterItem = true;
                }
                else
                {
                    this.listView1.InsertionMark.AppearsAfterItem = false;
                }
            }

            // Set the location of the insertion mark. If the mouse is
            // over the dragged item, the targetIndex value is -1 and
            // the insertion mark disappears.
            this.listView1.InsertionMark.Index = targetIndex;

            Console.WriteLine("listView1_DragOver");
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            Console.WriteLine("listView1_DragEnter");

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            } else
            {
                e.Effect = DragDropEffects.Move;
            }

        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            Console.WriteLine("listView1_DragDrop");
            // Retrieve the index of the insertion mark;
            int targetIndex = this.listView1.InsertionMark.Index;

            // If the insertion mark is not visible, exit the method.
            if (targetIndex == -1)
            {
                return;
            }

            // If the insertion mark is to the right of the item with
            // the corresponding index, increment the target index.
            if (this.listView1.InsertionMark.AppearsAfterItem)
            {
                targetIndex++;
            }

            // Retrieve the dragged item.

            ListViewItem draggedItem = null;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                draggedItem = new ListViewItem(
                    filePaths[0]
                 );
                this.listView1.Items.Insert(targetIndex, draggedItem);
                this.imageList1.Images.Add(Icon.ExtractAssociatedIcon(filePaths[0]));
                this.imageList2.Images.Add(Icon.ExtractAssociatedIcon(filePaths[0]));
                draggedItem.ImageIndex = this.imageIndex++;
            }
            else
            {
                draggedItem =
              (ListViewItem)e.Data.GetData(typeof(ListViewItem));

                // Insert a copy of the dragged item at the target index.
                // A copy must be inserted before the original item is removed
                // to preserve item index values.
                this.listView1.Items.Insert(
                    targetIndex, (ListViewItem)draggedItem.Clone());

                // Remove the original copy of the dragged item.
                this.listView1.Items.Remove(draggedItem);
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
         
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.Activate();
            if (!this.Visible)
            {
                this.Visible = true;
            }
            else
            {
                this.Visible = false;
            }
        }
    }

    // Sorts ListViewItem objects by index.
    class ListViewIndexComparer : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            return ((ListViewItem)x).Index - ((ListViewItem)y).Index;
        }
    }
}
