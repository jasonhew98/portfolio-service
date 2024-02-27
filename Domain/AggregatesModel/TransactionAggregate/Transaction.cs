using Domain.Model;
using Domain.Seedwork;
using System;
using System.Collections.Generic;

namespace Domain.AggregatesModel.TransactionAggregate
{
    public enum PaymentMethod
    {
        Cash,
        DebitCard,
        CreditCard,
        Transfer,
    }


    public class Transaction : AuditableEntity, IAggregateRoot
    {
        public string TransactionId { get; private set; }
        public string MainCategory { get; private set; }
        public string SubCategory { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public string Notes { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public double PaymentAmount { get; private set; }

        public Transaction(
            string transactionId,
            string mainCategory,
            string subCategory,
            DateTime transactionDate,
            string notes,
            PaymentMethod paymentMethod,
            double paymentAmount,
            string createdBy = null,
            string createdByName = null,
            DateTime? createdUTCDateTime = null,
            string modifiedBy = null,
            string modifiedByName = null,
            DateTime? modifiedUTCDateTime = null)
            : base(
                  createdBy: createdBy,
                  createdByName: createdByName,
                  createdUTCDateTime: createdUTCDateTime,
                  modifiedBy: modifiedBy,
                  modifiedByName: modifiedByName,
                  modifiedUTCDateTime: modifiedUTCDateTime)
        {
            TransactionId = transactionId;
            MainCategory = mainCategory;
            SubCategory = subCategory;
            TransactionDate = transactionDate;
            Notes = notes;
            PaymentMethod = paymentMethod;
            PaymentAmount = paymentAmount;
        }

        public void UpdateTransactionDetails(
            string mainCategory,
            string subCategory,
            DateTime transactionDate,
            string notes,
            PaymentMethod paymentMethod,
            double paymentAmount)
        {
            MainCategory = mainCategory;
            SubCategory = subCategory;
            TransactionDate = transactionDate;
            Notes = notes;
            PaymentMethod = paymentMethod;
            PaymentAmount = paymentAmount;
        }
    }
}
