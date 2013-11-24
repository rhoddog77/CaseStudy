﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaseStudy.Base
{
    public class Validator
    {
        private static string _title = "Entry Error";

        public static string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public static bool IsPresent(Object text)
        {
            if (text is TextBox)
            {
                TextBox textBox = (TextBox)text;
                if (textBox.Text == "")
                {
                    MessageBox.Show(string.Format("{0} is a required field.", textBox.Tag), Title);
                    textBox.Focus();
                    return false;
                }
            }
            else if (text is RichTextBox)
            {
                RichTextBox richTextBox = (RichTextBox)text;
                if (richTextBox.Text == "")
                {
                    MessageBox.Show(string.Format("{0} is a required field.", richTextBox.Tag), Title);
                    richTextBox.Focus();
                    return false;
                }
            }
            return true;
        }


        public static bool IsInt(TextBox textBox)
        {
            try
            {
                Convert.ToInt32(textBox.Text);
                return true;
            }
            catch (FormatException)
            {
                MessageBox.Show(string.Format("{0} must be an integer number.", textBox.Tag), Title);
                textBox.Focus();
                return false;
            }
        }
    }
}
