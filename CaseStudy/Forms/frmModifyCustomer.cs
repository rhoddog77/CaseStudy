﻿using CaseStudy.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaseStudy.Forms
{
    public partial class frmModifyCustomer : frmNewCustomer
    {
        private Customer _customerToModify = null;
        public frmModifyCustomer(Customer customer)
        {
            _customerToModify = customer;
            InitializeComponent();
            base.btnSubmit.Click -= new System.EventHandler(base.btnSubmit_Click);
            base.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            FillFormData();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (base.IsValidData())
            {
                _customerToModify.FirstName = txtFirstName.Text;
                _customerToModify.LastName = txtLastName.Text;
                _customerToModify.DateOfBirth = dateDOB.Value;
                Customer.Types type = (Customer.Types)Enum.Parse(typeof(Customer.Types), comboType.SelectedItem.ToString());
                _customerToModify.Type = type;
                if (chkResponsibleParty.Checked)
                {
                    _customerToModify.PersonType = Person.PersonTypes.ResponsibleParty;
                }
                else
                {
                    _customerToModify.PersonType = Person.PersonTypes.Customer;
                    //get address from responsible party
                    //add customer as dependant of responsible party
                }
                _customerToModify.Address.Street = txtStreet.Text;
                _customerToModify.Address.City = txtCity.Text;
                _customerToModify.Address.StateCode = txtState.Text;
                _customerToModify.Address.Zip = int.Parse(txtZip.Text);

                base.customer = _customerToModify;

                base.Close();
            }
        }

        private void FillFormData()
        {
            if (_customerToModify != null)
            {
                base.txtFirstName.Text = _customerToModify.FirstName;
                base.txtLastName.Text = _customerToModify.LastName;
                base.dateDOB.Value = (DateTime)_customerToModify.DateOfBirth;
                base.comboType.SelectedItem = _customerToModify.Type;
                if (_customerToModify.PersonType == Person.PersonTypes.ResponsibleParty)
                {
                    base.chkResponsibleParty.CheckState = CheckState.Checked;
                    base.UpdateEnabledControls(true);
                }
                else
                {
                    base.chkResponsibleParty.CheckState = CheckState.Unchecked;
                    base.UpdateEnabledControls(false);
                }
                base.txtStreet.Text = _customerToModify.Address.Street;
                base.txtCity.Text = _customerToModify.Address.City;
                base.txtState.Text = _customerToModify.Address.StateCode;
                base.txtZip.Text = _customerToModify.Address.Zip.ToString();
            }
        }

        
    }
}