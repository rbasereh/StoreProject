namespace TP.Domain.Common;

public class Constants
{
    public const string FileIsNotSelectedOrEmpty = "file_is_not_selected_or_empty";
    public const string PropIsRequired = "{0}IsRequired";
    public const string RecordNotFound = "{0}RecordNotFound";
    public const string RecordIsDuplicated = "{0}RecordIsDuplicated";
    public const string OtpIsInvalid = "OtpIsInvalid";
    public const string OtpExpired = "OtpExpired";
    public const string UserNameOrPasswordIsInvalid = "UserOrPasswordIsInvalid";
    public const string FileIsInvalid = "FileIsInvalid";
    public const string StatementDateTimeFormat = "yyyy MMM d, HH:mm";

}
public class StatementTransactionType
{
    public const string Buy = "buy";
    public const string Sell = "sell";
    public const string Balance = "balance";
}

