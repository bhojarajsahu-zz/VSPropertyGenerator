using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Utilities;

namespace bhojarajsahu88.VSPropertyGenerator
{
    public partial class PropertyGenerator : Form
    {
        string fileLoc = string.Empty;
        XmlDocument newDocument = new XmlDocument();
        List<string> elementList;
        public PropertyGenerator()
        {
            InitializeComponent();
        }

        private void buttonLoadXML_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialogXML.ShowDialog();
            fileLoc = openFileDialogXML.FileName;
            if (result == DialogResult.OK) // Test result.
            {
                toolStripStatusLabelStatus.Text = "Status";
                textBoxDestination.Text = "";
                labelFile.Text = fileLoc;
                if (fileLoc.Contains(".txt"))
                {
                    try
                    {
                        string fileContent = File.ReadAllText(fileLoc);
                        textBoxSource.Text = fileContent;
                    }
                    catch
                    {
                        MessageBox.Show("Can not load the file.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    }

                }
                else if (fileLoc.Contains(".xml"))
                {
                    string xmlRawText = string.Empty;
                    try
                    {
                        try
                        {
                            textBoxSource.Text = XmlOperation.LoadFileToXML(fileLoc, out newDocument);
                            textBoxSource.ForeColor = Color.Black;
                        }
                        catch { }
                    }
                    catch
                    {
                        textBoxSource.Text = xmlRawText;
                        textBoxSource.ForeColor = Color.Red;
                    }

                }
                else
                {
                    MessageBox.Show("File format not supported. Please upload XML or TXT file.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                }
            }
        }
        public void processXMLNodes(XmlNode xmlNode)
        {
            try
            {
                if (xmlNode.HasChildNodes)
                {
                    XmlNodeList headerNodeList = xmlNode.ChildNodes;
                    if (headerNodeList != null)
                    {
                        foreach (XmlNode headerAttribute in headerNodeList)
                        {
                            processXMLNodes(headerAttribute);
                        }
                    }
                }
                else
                {
                    if (xmlNode.Attributes != null)
                    {
                        toolStripStatusLabelStatus.Text = "Status";
                        XmlNode namedItem = xmlNode.Attributes.GetNamedItem(textBoxElement.Text);//("ID");
                        elementList.Add(namedItem.InnerText.Replace(' ', '_').Trim());
                    }
                    else
                    {
                        toolStripStatusLabelStatus.Text = "Element not applied to this XML file";
                        elementList.Add(xmlNode.ParentNode.Name.Replace(' ', '_').Trim());
                    }
                }
            }
            catch { }
        }
        public void generateCode()
        {
            try
            {
                string am = comboBoxAM.Text;
                string returnType = comboBoxReturn.Text;
                string getSet = string.Empty;
                if (radioButtonNormal.Checked)
                {
                    if (comboBoxProperty.Text == "GET/SET")
                        getSet = "{get; set;}";
                    else if (comboBoxProperty.Text == "GET")
                        getSet = "{get;}";
                    if (comboBoxProperty.Text == "SET")
                        getSet = "{set;}";
                    foreach (string item in elementList)
                    {
                        string formattedProperty = "\r\t" + am + " " + returnType + " " + XmlOperation.LowercaseFirst(item) + " " + getSet;
                        textBoxDestination.Text = textBoxDestination.Text + Environment.NewLine + formattedProperty;
                    }
                }
                else
                {
                    foreach (string item in elementList)
                    {
                        string shortExp = XmlOperation.LowercaseFirst(item);
                        string longExp = XmlOperation.UppercaseFirst(item);

                        if (shortExp == longExp)
                        {
                            shortExp = "_" + shortExp;
                            if (comboBoxProperty.Text == "GET/SET")
                                getSet = Environment.NewLine + "\r\t" + "{" + Environment.NewLine + "\r\t" + "\r\t" + "get { return " + shortExp + "; }" + Environment.NewLine + "\r\t" + "\r\t" + "set {" + shortExp + " = value; }" + Environment.NewLine + "\r\t" + "}";
                            else if (comboBoxProperty.Text == "GET")
                                getSet = Environment.NewLine + "\r\t" + "{" + Environment.NewLine + "\r\t" + "\r\t" + "get { return " + shortExp + "; }" + Environment.NewLine + "\r\t" + "}";
                            if (comboBoxProperty.Text == "SET")
                                getSet = Environment.NewLine + "\r\t" + "{" + Environment.NewLine + "\r\t" + "\r\t" + "set {" + shortExp + " = value; }" + Environment.NewLine + "\r\t" + "}";

                        }
                        else
                        {
                            if (comboBoxProperty.Text == "GET/SET")
                                getSet = Environment.NewLine + "\r\t" + "{" + Environment.NewLine + "\r\t" + "\r\t" + "get { return " + shortExp + "; }" + Environment.NewLine + "\r\t" + "\r\t" + "set {" + shortExp + " = value; }" + Environment.NewLine + "\r\t" + "}";
                            else if (comboBoxProperty.Text == "GET")
                                getSet = Environment.NewLine + "\r\t" + "{" + Environment.NewLine + "\r\t" + "\r\t" + "get { return " + shortExp + "; }" + Environment.NewLine + "\r\t" + "}";
                            if (comboBoxProperty.Text == "SET")
                                getSet = Environment.NewLine + "\r\t" + "{" + Environment.NewLine + "\r\t" + "\r\t" + "set {" + shortExp + " = value; }" + Environment.NewLine + "\r\t" + "}";

                        }
                        string formattedProperty = "\r\t" + am + " " + returnType + " " + shortExp + ";" + Environment.NewLine;
                        formattedProperty = formattedProperty + "\r\t" + am + " " + returnType + " " + longExp + " \n" + getSet;
                        textBoxDestination.Text = textBoxDestination.Text + Environment.NewLine + formattedProperty;
                    }
                }
            }
            catch { }

        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            elementList = new List<string>();
            toolStripStatusLabelStatus.Text = "Status";

            if (checkBoxGenerateFromText.Checked)
            {
                if (comboBoxAM.SelectedIndex == -1 || comboBoxProperty.SelectedIndex == -1 || comboBoxReturn.SelectedIndex == -1 || textBoxClass.Text == "" || textBoxElement.Text == "")
                    MessageBox.Show("Please load a file using load XML. Also select \"Access Specifier, Return Type, Property Type, Class Name & Element for Property\" for the same.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                else
                {
                    if (!string.IsNullOrEmpty(textBoxSource.Text))
                    {
                        string fileContent = textBoxSource.Text;
                        //textBoxSource.Text = fileContent;
                        fileContent.Replace('.', ' ');
                        string[] elementItemList = fileContent.Split(' ');
                        foreach (string item in elementItemList)
                        {
                            string element = item.Trim();
                            if (!string.IsNullOrWhiteSpace(element))
                                elementList.Add(item);
                        }
                        textBoxDestination.Text = comboBoxAM.Text + " " + "class " + textBoxClass.Text + "\r\n" + "{";
                        generateCode();
                        textBoxDestination.Text = textBoxDestination.Text + "\r\n" + "}";

                    }
                    else
                        MessageBox.Show("Please enter element name in the text area.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                }
            }
            else
            {

                if (comboBoxAM.SelectedIndex == -1 || comboBoxProperty.SelectedIndex == -1 || comboBoxReturn.SelectedIndex == -1 || labelFile.Text == "" || textBoxClass.Text == "" || textBoxElement.Text == "")
                    MessageBox.Show("Please load a file using load XML. Also select \"Access Specifier, Return Type, Property Type, Class Name & Element for Property\" for the same.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                else
                {
                    try
                    {
                        if (fileLoc.Contains(".txt"))
                        {
                            try
                            {
                                string fileContent = File.ReadAllText(fileLoc);
                                //textBoxSource.Text = fileContent;
                                fileContent.Replace('.', ' ');
                                string[] elementItemList = fileContent.Split(' ');
                                foreach (string item in elementItemList)
                                {
                                    string element = item.Trim();
                                    if (!string.IsNullOrWhiteSpace(element))
                                        elementList.Add(item);
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Can not load the file.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                            }

                        }
                        else if (fileLoc.Contains(".xml"))
                        {
                            try
                            {
                                XmlDocument docIn = new XmlDocument();
                                docIn.Load(fileLoc);

                                XmlNodeList headerNodeList = docIn.DocumentElement.ChildNodes;
                                //XmlNodeList headerNodeList = docIn.SelectSingleNode("/GROUP").ChildNodes;
                                if (headerNodeList != null)
                                {
                                    foreach (XmlNode headerAttribute in headerNodeList)
                                    {
                                        processXMLNodes(headerAttribute);
                                    }
                                }
                            }
                            catch { }
                        }
                        textBoxDestination.Text = comboBoxAM.Text + " " + "class " + textBoxClass.Text + "\r\n" + "{";
                        generateCode();
                        textBoxDestination.Text = textBoxDestination.Text + "\r\n" + "}";
                    }
                    catch { }
                }
            }
            this.Cursor = Cursors.Default;
        }

        private void PropertyGeneratorFromXML_Load(object sender, EventArgs e)
        {
            labelFile.Text = "";
            comboBoxAM.SelectedIndex = 0;
            comboBoxProperty.SelectedIndex = 0;
            comboBoxReturn.SelectedIndex = 0;
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxDestination.Text))
                Clipboard.SetText(textBoxDestination.Text, TextDataFormat.Text);
        }

        private void buttonUIDesign_Click(object sender, EventArgs e)
        {
        }

        private void checkBoxGenerateFromText_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxGenerateFromText.Checked)
            {
                textBoxSource.ReadOnly = false;
                textBoxSource.Text = "";
                labelFile.Text = "";
            }
            else
                textBoxSource.ReadOnly = true;
        }
    }
}
