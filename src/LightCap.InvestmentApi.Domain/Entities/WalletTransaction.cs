using LightCap.InvestmentApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LightCap.InvestmentApi.Domain.Entities.Wallets;

namespace LightCap.InvestmentApi.Domain.Entities
{
    public class WalletTransaction
    {
      
        // The ledger - an append-only history of every single wallet movement,
        // across BOTH balances. This is the source of truth; Wallet's two balance
        // fields are just cached "current totals" for fast reads.
        
            public Guid Id { get; set; }
            public Guid WalletId { get; set; }
            public Guid UserId { get; set; }   // denormalized for easy querying without a join

            public WalletTransactionType Type { get; set; }
            public WalletTransactionStatus Status { get; set; }           
            public decimal Amount { get; set; }

            // Snapshots for audit trail - what each balance was AFTER this transaction.
            public decimal PendingRoundUpBalanceAfter { get; set; }
            public decimal AvailableBalanceAfter { get; set; }

            // Ties this transaction back to its source, and is critical for idempotency:
            // e.g. the Mono transaction ID for a SpendRoundUp, or the Mono DirectPay
            // reference for a ThresholdDebit. This is what stops the same webhook
            // from being processed twice.
            public string SourceReference { get; set; } = string.Empty;
            public string? Description { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? CompletedAt { get; set; }
            public Wallet? Wallet { get; set; }
        
    }
}



