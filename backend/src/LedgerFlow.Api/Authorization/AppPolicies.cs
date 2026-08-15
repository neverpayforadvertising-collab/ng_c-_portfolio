namespace LedgerFlow.Api.Authorization;

public static class AppPolicies
{
    public const string CanViewCustomers =
        nameof(CanViewCustomers);

    public const string CanManageCustomers =
        nameof(CanManageCustomers);

    public const string CanDeactivateCustomers =
        nameof(CanDeactivateCustomers);

    public const string CanViewInvoices =
        nameof(CanViewInvoices);

    public const string CanManageInvoices =
        nameof(CanManageInvoices);

    public const string CanRecordPayments =
        nameof(CanRecordPayments);

    public const string CanViewExpenses =
        nameof(CanViewExpenses);

    public const string CanManageExpenses =
        nameof(CanManageExpenses);

    public const string CanViewReports =
        nameof(CanViewReports);

    public const string CanManageUsers =
        nameof(CanManageUsers);
}