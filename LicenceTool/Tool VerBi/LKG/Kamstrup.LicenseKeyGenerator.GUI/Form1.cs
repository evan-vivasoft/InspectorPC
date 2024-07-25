using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Kamstrup.LicenseKeyGenerator.Model;
using Kamstrup.LicenseKeyGenerator.Controller;

namespace Kamstrup.LicenseKeyGenerator.GUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        /// <summary>
        /// This method initializes the GUI for Pc Base
        /// </summary>
        private void initPcBase ()
        {
            label_featureList.Text = "Measuring Points";
            label_featureList.Visible = true;
            radioButton_feature1.Text = "500 MP";
            radioButton_feature1.Visible = true;
            radioButton_feature2.Text = "1000 MP";
            radioButton_feature2.Visible = true;
            radioButton_feature3.Text = "2500 MP";
            radioButton_feature3.Visible = true;
            radioButton_feature4.Text = "5000 MP";
            radioButton_feature4.Visible = true;
            radioButton_feature5.Text = "10000 MP";
            radioButton_feature5.Visible = true;
            radioButton_feature6.Text = "30000 MP";
            radioButton_feature6.Visible = true;
            radioButton_feature7.Text = "60000 MP";
            radioButton_feature7.Visible = true;
        }
        private void initPcBase(FeatureItem item)
        {
            initPcBase();
            if (item.OrderNo == "6697-052")
                radioButton_feature1.Checked = true;
            else if (item.OrderNo == "6697-053")
                radioButton_feature2.Checked = true;
            else if (item.OrderNo == "6697-054")
                radioButton_feature3.Checked = true;
            else if (item.OrderNo == "6697-055")
                radioButton_feature4.Checked = true;
            else if (item.OrderNo == "6697-056")
                radioButton_feature5.Checked = true;
            else if (item.OrderNo == "6697-057")
                radioButton_feature6.Checked = true;
            else if (item.OrderNo == "6697-058")
                radioButton_feature7.Checked = true;
            else
                MessageBox.Show("Ordre nummeret kunne ikke findes: " + item.OrderNo);
                        
        }


        /// <summary>
        /// 
        /// This method initializes the GUI for PC Term Pro
        /// </summary>
        private void initPcTermPro()
        {
        }

        private void radioButton_pcbase_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_pcbase.Checked)
            {
                initPcBase();
            }
            else if (radioButton_pcTermPro.Checked)
            {
                initPcTermPro();
            }
        }

        private void button_search_Click(object sender, EventArgs e)
        {
            OrderItemController controller = OrderItemController.Instance;
            OrderItemLookupObject obj = controller.LookupStoredItem(textBox_itemNumber.Text);
            initializeGui(obj);
           
        }
        private void initializeGui(OrderItemLookupObject obj)
        {
            if (obj != null)
            {
                if (obj.Product.Product == ProductItem.product.PcBase)
                {
                    radioButton_pcbase.Checked = true;
                    initPcBase(obj.Feature);
                }


                else if (obj.Product.Product == ProductItem.product.PcTermPro)
                {
                    radioButton_pcbase.Checked = true;
                }
            }
            else
                MessageBox.Show("Ordre nummer kunne ikke findes");
        }

        private void button_reset_Click(object sender, EventArgs e)
        {
            label_featureList.Visible = false;
            radioButton_pcbase.Checked = false;
            radioButton_feature1.Visible = false;
            radioButton_feature1.Checked = false;
            radioButton_feature2.Visible = false;
            radioButton_feature2.Checked = false;
            radioButton_feature3.Visible = false;
            radioButton_feature3.Checked = false;
            radioButton_feature4.Visible = false;
            radioButton_feature4.Checked = false;
            radioButton_feature5.Visible = false;
            radioButton_feature5.Checked = false;
            radioButton_feature6.Visible = false;
            radioButton_feature6.Checked = false;
            radioButton_feature7.Visible = false;
            radioButton_feature7.Checked = false;
        }

    }
}
