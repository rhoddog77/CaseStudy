﻿using CaseStudy.Business;
using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaseStudy.DataAccess
{
    public static class CustomerDB
    {
        public static List<Customer> GetCustomers(bool onlyResponsibleParty)
        {
            StringBuilder _query = new StringBuilder("SELECT * FROM Customer C " +
                                                     "JOIN Person P ON P.PersonID = C.PersonID " +
                                                     "JOIN Address A ON A.AddressID = P.AddressID ");
            if(onlyResponsibleParty)
            {
                _query.Append("WHERE P.PersonType = 'ResponsibleParty'");
            }

            return GetCustomersFromDB(_query.ToString());
        }

        public static Customer GetCustomerByID(long customerID)
        {
            string _query = string.Format("SELECT * FROM Customer C " +
                "JOIN Person P ON P.PersonID = C.PersonID " +
                "JOIN Address A ON A.AddressID = P.AddressID " +
                "WHERE C.CustomerID = {0}", customerID);
            return GetCustomersFromDB(_query).FirstOrDefault();
        }

        public static List<Customer> GetDependents(long? responsiblePartyID)
        {
            string _query = string.Format("SELECT * FROM Customer C " +
                            "JOIN Person P ON P.PersonID = C.PersonID " +
                            "JOIN Address A ON A.AddressID = P.AddressID " +
                            "WHERE C.ResponsiblePartyID = {0}", responsiblePartyID);
            return GetCustomersFromDB(_query);
        }

        private static List<Customer> GetCustomersFromDB(string _query)
        {
            List<Customer> customers = new List<Customer>();
            try
            {
                using(SqlCeDataReader reader = CaseStudyDB.ExecuteReader(_query.ToString()))
                {
                    while (reader != null & reader.Read())
                    {
                        Customer customer = GetCustomerFromReader(reader);
                        if (customer != null)
                        {
                            customers.Add(customer);
                        }
                        else
                        {
                            throw new Exception("Can't create Customer from DB");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(string.Format("Error Getting Customers. {0}", ex.Message));
            }
            return customers;
        }

        public static long? AddCustomer(Customer customer)
        {
            if(customer.PersonType == Person.PersonTypes.ResponsibleParty)
            {
                customer.Address.AddressID = AddressDB.AddAddress(customer.Address);
            }
            customer.PersonID =  PersonDB.AddPerson(customer);
            string _query = string.Format("INSERT INTO Customer (PersonID, ResponsiblePartyID, Type) " +
                "VALUES({0}, {1}, '{2}')", customer.PersonID, customer.ResponsiblePartyID == null? 0 : customer.ResponsiblePartyID, 
                customer.PersonType);
            return CaseStudyDB.ExecuteNonQuery(_query);
        }

        public static void ModifyCustomer(Customer customer)
        {
            if(customer.PersonType == Person.PersonTypes.ResponsibleParty)
            {
                AddressDB.UpdateAddress(customer.Address);
            }
            PersonDB.UpdatePerson(customer);

            string _query = string.Format("UPDATE Customer " +
                "SET ResponsiblePartyID={0}, Type='{1}' WHERE CustomerID = {2}",
                customer.ResponsiblePartyID == null ? 0 : customer.ResponsiblePartyID,
                customer.PersonType, customer.CustomerID);

            CaseStudyDB.ExecuteNonQuery(_query);
        }

        public static void DeleteCustomer(Customer customer)
        {
            if(customer.PersonType == Person.PersonTypes.ResponsibleParty)
            {
                AddressDB.DeleteAddress(customer.Address.AddressID);
            }

            TransactionDB.DeleteTransactionsByCustomer((long)customer.CustomerID);
            PersonDB.DeletePerson(customer.PersonID);

            string _query = string.Format("DELETE FROM Customer WHERE CustomerID = {0}", customer.CustomerID);
            CaseStudyDB.ExecuteNonQuery(_query);
        }

        public static Customer GetCustomerFromReader(SqlCeDataReader reader)
        {
            if (reader != null)
            {
                Customer customer = new Customer();
                customer.CustomerID = (long)reader["CustomerID"];
                if (!reader.IsDBNull(reader.GetOrdinal("ResponsiblePartyID")))
                {
                    customer.ResponsiblePartyID = (long)reader["ResponsiblePartyID"];

                }
                Customer.Types type;
                Enum.TryParse<Customer.Types>(reader["Type"].ToString(), out type);
                customer.Type = type;
                customer.PersonID = (long)reader["PersonID"];
                customer.FirstName = reader["FirstName"].ToString();
                customer.LastName = reader["LastName"].ToString();
                customer.DateOfBirth = DateTime.Parse(reader["DateOfBirth"].ToString());
                Person.PersonTypes personType;
                Enum.TryParse<Person.PersonTypes>(reader["PersonType"].ToString(), out personType);
                customer.PersonType = personType;
                customer.Email = reader["Email"].ToString();
                customer.Password = reader["Password"].ToString();
                customer.SetAddress(AddressDB.GetAddressFromReader(reader));

                return customer;
            }
            return null;
        }

    }
}
