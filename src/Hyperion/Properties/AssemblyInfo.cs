using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;


#if UNSAFE
[assembly: AllowPartiallyTrustedCallers]
[assembly: SecurityTransparent]
[assembly: SecurityRules(SecurityRuleSet.Level1, SkipVerificationInFullTrust = true)]
#endif