using System;
using System.Runtime.Serialization;
using MessagePack;

namespace PerfBenchmark.Types
{
  public enum MaritalStatus
  {
    Married,

    Divorced,

    HatesAll
  }

  [MessagePackObject]
  [Serializable]
  public class TypicalPersonData
  {
    /// <summary>
    /// Required by some serilizers (i.e. XML)
    /// </summary>
    public TypicalPersonData() { }

    [Key(0)]
    public virtual string Address1 { get; set; }

    [Key(1)]
    public virtual string Address2 { get; set; }

    [Key(2)]
    public virtual string AddressCity { get; set; }

    [Key(3)]
    public virtual string AddressState { get; set; }

    [Key(4)]
    public virtual string AddressZip { get; set; }

    [Key(5)]
    public virtual double CreditScore { get; set; }

    [Key(6)]
    public virtual DateTime DOB { get; set; }

    [Key(7)]
    public virtual string EMail { get; set; }

    [Key(8)]
    public virtual string FirstName { get; set; }

    [Key(9)]
    public virtual string HomePhone { get; set; }

    [Key(10)]
    public virtual string LastName { get; set; }

    [Key(11)]
    public virtual MaritalStatus MaritalStatus { get; set; }

    [Key(12)]
    public virtual string MiddleName { get; set; }

    [Key(13)]
    public virtual string MobilePhone { get; set; }

    [Key(14)]
    public virtual bool RegisteredToVote { get; set; }

    [Key(15)]
    public virtual decimal Salary { get; set; }

    [Key(16)]
    public virtual int YearsOfService { get; set; }

    [Key(17)]
    public virtual string SkypeID { get; set; }

    [Key(18)]
    public virtual string YahooID { get; set; }

    [Key(19)]
    public virtual string GoogleID { get; set; }

    [Key(20)]
    public virtual string Notes { get; set; }

    [Key(21)]
    public virtual bool? IsSmoker { get; set; }

    [Key(22)]
    public virtual bool? IsLoving { get; set; }

    [Key(23)]
    public virtual bool? IsLoved { get; set; }

    [Key(24)]
    public virtual bool? IsDangerous { get; set; }

    [Key(25)]
    public virtual bool? IsEducated { get; set; }

    [Key(26)]
    public virtual DateTime? LastSmokingDate { get; set; }

    [Key(27)]
    public virtual decimal? DesiredSalary { get; set; }

    [Key(28)]
    public virtual double? ProbabilityOfSpaceFlight { get; set; }

    [Key(29)]
    public virtual int? CurrentFriendCount { get; set; }

    [Key(30)]
    public virtual int? DesiredFriendCount { get; set; }

    private static int counter;

    public static TypicalPersonData MakeRandom()
    {
      var rnd = counter++;

      var data = new TypicalPersonData
      {
        FirstName = NaturalTextGenerator.GenerateFirstName(),
        MiddleName = NaturalTextGenerator.GenerateFirstName(),
        LastName = NaturalTextGenerator.GenerateLastName(),
        DOB = DateTime.Now.AddYears(5),
        Salary = 55435345,
        YearsOfService = 25,
        CreditScore = 0.7562,
        RegisteredToVote = (DateTime.UtcNow.Ticks & 1) == 0,
        MaritalStatus = MaritalStatus.HatesAll,
        Address1 = NaturalTextGenerator.GenerateAddressLine(),
        Address2 = NaturalTextGenerator.GenerateAddressLine(),
        AddressCity = NaturalTextGenerator.GenerateCityName(),
        AddressState = "CA",
        AddressZip = "91606",
        HomePhone = (DateTime.UtcNow.Ticks & 1) == 0 ? "(555) 123-4567" : null,
        EMail = NaturalTextGenerator.GenerateEMail()
      };

      if (0 != (rnd & (1 << 32))) data.Notes = NaturalTextGenerator.Generate(45);
      if (0 != (rnd & (1 << 31))) data.SkypeID = NaturalTextGenerator.GenerateEMail();
      if (0 != (rnd & (1 << 30))) data.YahooID = NaturalTextGenerator.GenerateEMail();

      if (0 != (rnd & (1 << 29))) data.IsSmoker = 0 != (rnd & (1 << 17));
      if (0 != (rnd & (1 << 28))) data.IsLoving = 0 != (rnd & (1 << 16));
      if (0 != (rnd & (1 << 27))) data.IsLoved = 0 != (rnd & (1 << 15));
      if (0 != (rnd & (1 << 26))) data.IsDangerous = 0 != (rnd & (1 << 14));
      if (0 != (rnd & (1 << 25))) data.IsEducated = 0 != (rnd & (1 << 13));

      if (0 != (rnd & (1 << 24))) data.LastSmokingDate = DateTime.Now.AddYears(-10);

      if (0 != (rnd & (1 << 23))) data.DesiredSalary = rnd / 1000m;
      if (0 != (rnd & (1 << 22))) data.ProbabilityOfSpaceFlight = rnd / (double)int.MaxValue;

      if (0 != (rnd & (1 << 21)))
      {
        data.CurrentFriendCount = rnd % 123;
        data.DesiredFriendCount = rnd % 121000;
      }

      return data;
    }
  }

  public class NaturalTextGenerator
  {
    public static string GenerateEMail()
    {
      return "foo@fooo.com";
    }

    public static string Generate(int i)
    {
      return "fskldjflksjfl ksj dlfkjsdfl ksdjklf jsdlkj" + DateTime.Now.Ticks;
    }

    public static string GenerateAddressLine()
    {
      return "fkjdskfjskfjs" + DateTime.Now.Ticks;
    }

    public static string GenerateFirstName()
    {
      return "fksjdfkjsdkfjksdfs" + DateTime.Now.Ticks;
    }

    public static string GenerateCityName()
    {
      return "fksdfkjsdkfjsdkfs";
    }

    public static string GenerateLastName()
    {
      return "kfjdskdfjskj";
    }
  }

  public class ExternalRandomGenerator
  {
  }
}