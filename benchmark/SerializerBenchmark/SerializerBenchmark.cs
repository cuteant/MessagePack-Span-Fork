extern alias newmsgpackcore;

namespace Benchmark
{
    using Benchmark.Fixture;
    using Benchmark.Models;
    using Benchmark.Serializers;
    using BenchmarkDotNet.Attributes;
    using System;
    using System.Buffers.Binary;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [Config(typeof(BenchmarkConfig))]
    public class AllSerializerBenchmark_BytesInOut
    {
        [ParamsSource(nameof(Serializers))]
        public SerializerBase Serializer;

        // Currently BenchmarkdDotNet does not detect inherited ParamsSource so use copy and paste:)

        public IEnumerable<SerializerBase> Serializers => new SerializerBase[]
        {
            new MessagePack_Official(),
            new MessagePack_Span(),
            new Typeless_Official(),
            new Typeless_Span(),
            //new MessagePackLz4_Official(),
            //new MessagePackLz4_Span(),
            new ProtobufNet(),

            //new JsonNet(),
            //new BinaryFormatter_(),
            //new DataContract_(),
            new Hyperion_(),
            //new Jil_(),
            new SpanJson_(),
            new Utf8Json_(),
            //new MsgPackCli(),
            //new FsPickler_(),
            //new Ceras_(),
        };

        protected static readonly ExpressionTreeFixture ExpressionTreeFixture = new ExpressionTreeFixture();

        // primitives

        protected static readonly sbyte SByteInput = ExpressionTreeFixture.Create<sbyte>();
        protected static readonly short ShortInput = ExpressionTreeFixture.Create<short>();
        protected static readonly int IntInput = ExpressionTreeFixture.Create<int>();
        protected static readonly long LongInput = ExpressionTreeFixture.Create<long>();
        protected static readonly byte ByteInput = ExpressionTreeFixture.Create<byte>();
        protected static readonly ushort UShortInput = ExpressionTreeFixture.Create<ushort>();
        protected static readonly uint UIntInput = ExpressionTreeFixture.Create<uint>();
        protected static readonly ulong ULongInput = ExpressionTreeFixture.Create<ulong>();
        protected static readonly bool BoolInput = ExpressionTreeFixture.Create<bool>();
        protected static readonly string StringInput = ExpressionTreeFixture.Create<string>();
        protected static readonly char CharInput = ExpressionTreeFixture.Create<char>();
        protected static readonly DateTime DateTimeInput = ExpressionTreeFixture.Create<DateTime>();
        protected static readonly Guid GuidInput = ExpressionTreeFixture.Create<Guid>();
        protected static readonly byte[] BytesInput = ExpressionTreeFixture.Create<byte[]>();

        // models

        protected static readonly Benchmark.Models.AccessToken AccessTokenInput = ExpressionTreeFixture.Create<Benchmark.Models.AccessToken>();

        protected static readonly Benchmark.Models.AccountMerge AccountMergeInput = ExpressionTreeFixture.Create<Benchmark.Models.AccountMerge>();

        protected static readonly Benchmark.Models.Answer AnswerInput = ExpressionTreeFixture.Create<Benchmark.Models.Answer>();

        protected static readonly Benchmark.Models.Badge BadgeInput = ExpressionTreeFixture.Create<Benchmark.Models.Badge>();

        protected static readonly Benchmark.Models.Comment CommentInput = ExpressionTreeFixture.Create<Benchmark.Models.Comment>();

        protected static readonly Benchmark.Models.Error ErrorInput = ExpressionTreeFixture.Create<Benchmark.Models.Error>();

        protected static readonly Benchmark.Models.Event EventInput = ExpressionTreeFixture.Create<Benchmark.Models.Event>();

        protected static readonly Benchmark.Models.MobileFeed MobileFeedInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileFeed>();

        protected static readonly Benchmark.Models.MobileQuestion MobileQuestionInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileQuestion>();

        protected static readonly Benchmark.Models.MobileRepChange MobileRepChangeInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileRepChange>();

        protected static readonly Benchmark.Models.MobileInboxItem MobileInboxItemInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileInboxItem>();

        protected static readonly Benchmark.Models.MobileBadgeAward MobileBadgeAwardInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileBadgeAward>();

        protected static readonly Benchmark.Models.MobilePrivilege MobilePrivilegeInput = ExpressionTreeFixture.Create<Benchmark.Models.MobilePrivilege>();

        protected static readonly Benchmark.Models.MobileCommunityBulletin MobileCommunityBulletinInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileCommunityBulletin>();

        protected static readonly Benchmark.Models.MobileAssociationBonus MobileAssociationBonusInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileAssociationBonus>();

        protected static readonly Benchmark.Models.MobileCareersJobAd MobileCareersJobAdInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileCareersJobAd>();

        protected static readonly Benchmark.Models.MobileBannerAd MobileBannerAdInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileBannerAd>();

        protected static readonly Benchmark.Models.MobileUpdateNotice MobileUpdateNoticeInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileUpdateNotice>();

        protected static readonly Benchmark.Models.FlagOption FlagOptionInput = ExpressionTreeFixture.Create<Benchmark.Models.FlagOption>();

        protected static readonly Benchmark.Models.InboxItem InboxItemInput = ExpressionTreeFixture.Create<Benchmark.Models.InboxItem>();

        protected static readonly Benchmark.Models.Info InfoInput = ExpressionTreeFixture.Create<Benchmark.Models.Info>();

        protected static readonly Benchmark.Models.NetworkUser NetworkUserInput = ExpressionTreeFixture.Create<Benchmark.Models.NetworkUser>();

        protected static readonly Benchmark.Models.Notification NotificationInput = ExpressionTreeFixture.Create<Benchmark.Models.Notification>();

        protected static readonly Benchmark.Models.Post PostInput = ExpressionTreeFixture.Create<Benchmark.Models.Post>();

        protected static readonly Benchmark.Models.Privilege PrivilegeInput = ExpressionTreeFixture.Create<Benchmark.Models.Privilege>();

        protected static readonly Benchmark.Models.Question QuestionInput = ExpressionTreeFixture.Create<Benchmark.Models.Question>();

        protected static readonly Benchmark.Models.QuestionTimeline QuestionTimelineInput = ExpressionTreeFixture.Create<Benchmark.Models.QuestionTimeline>();

        protected static readonly Benchmark.Models.Reputation ReputationInput = ExpressionTreeFixture.Create<Benchmark.Models.Reputation>();

        protected static readonly Benchmark.Models.ReputationHistory ReputationHistoryInput = ExpressionTreeFixture.Create<Benchmark.Models.ReputationHistory>();

        protected static readonly Benchmark.Models.Revision RevisionInput = ExpressionTreeFixture.Create<Benchmark.Models.Revision>();

        protected static readonly Benchmark.Models.SearchExcerpt SearchExcerptInput = ExpressionTreeFixture.Create<Benchmark.Models.SearchExcerpt>();

        protected static readonly Benchmark.Models.ShallowUser ShallowUserInput = ExpressionTreeFixture.Create<Benchmark.Models.ShallowUser>();

        protected static readonly Benchmark.Models.SuggestedEdit SuggestedEditInput = ExpressionTreeFixture.Create<Benchmark.Models.SuggestedEdit>();

        protected static readonly Benchmark.Models.Tag TagInput = ExpressionTreeFixture.Create<Benchmark.Models.Tag>();

        protected static readonly Benchmark.Models.TagScore TagScoreInput = ExpressionTreeFixture.Create<Benchmark.Models.TagScore>();

        protected static readonly Benchmark.Models.TagSynonym TagSynonymInput = ExpressionTreeFixture.Create<Benchmark.Models.TagSynonym>();

        protected static readonly Benchmark.Models.TagWiki TagWikiInput = ExpressionTreeFixture.Create<Benchmark.Models.TagWiki>();

        protected static readonly Benchmark.Models.TopTag TopTagInput = ExpressionTreeFixture.Create<Benchmark.Models.TopTag>();

        protected static readonly Benchmark.Models.User UserInput = ExpressionTreeFixture.Create<Benchmark.Models.User>();

        protected static readonly Benchmark.Models.UserTimeline UserTimelineInput = ExpressionTreeFixture.Create<Benchmark.Models.UserTimeline>();

        protected static readonly Benchmark.Models.WritePermission WritePermissionInput = ExpressionTreeFixture.Create<Benchmark.Models.WritePermission>();

        protected static readonly Benchmark.Models.MobileBannerAd.MobileBannerAdImage MobileBannerAdImageInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileBannerAd.MobileBannerAdImage>();

        protected static readonly Benchmark.Models.Info.Site SiteInput = ExpressionTreeFixture.Create<Benchmark.Models.Info.Site>();

        protected static readonly Benchmark.Models.Info.RelatedSite RelatedSiteInput = ExpressionTreeFixture.Create<Benchmark.Models.Info.RelatedSite>();

        protected static readonly Benchmark.Models.Question.ClosedDetails ClosedDetailsInput = ExpressionTreeFixture.Create<Benchmark.Models.Question.ClosedDetails>();

        protected static readonly Benchmark.Models.Question.Notice NoticeInput = ExpressionTreeFixture.Create<Benchmark.Models.Question.Notice>();

        protected static readonly Benchmark.Models.Question.MigrationInfo MigrationInfoInput = ExpressionTreeFixture.Create<Benchmark.Models.Question.MigrationInfo>();

        protected static readonly Benchmark.Models.User.BadgeCount BadgeCountInput = ExpressionTreeFixture.Create<Benchmark.Models.User.BadgeCount>();

        protected static readonly Benchmark.Models.Info.Site.Styling StylingInput = ExpressionTreeFixture.Create<Benchmark.Models.Info.Site.Styling>();

        protected static readonly Benchmark.Models.Question.ClosedDetails.OriginalQuestion OriginalQuestionInput = ExpressionTreeFixture.Create<Benchmark.Models.Question.ClosedDetails.OriginalQuestion>();

        object SByteOutput;
        object ShortOutput;
        object IntOutput;
        object LongOutput;
        object ByteOutput;
        object UShortOutput;
        object UIntOutput;
        object ULongOutput;
        object BoolOutput;
        object StringOutput;
        object CharOutput;
        object DateTimeOutput;
        object GuidOutput;
        object BytesOutput;

        object AccessTokenOutput;
        object AccountMergeOutput;
        object AnswerOutput;
        object BadgeOutput;
        object CommentOutput;
        object ErrorOutput;
        object EventOutput;
        object MobileFeedOutput;
        object MobileQuestionOutput;
        object MobileRepChangeOutput;
        object MobileInboxItemOutput;
        object MobileBadgeAwardOutput;
        object MobilePrivilegeOutput;
        object MobileCommunityBulletinOutput;
        object MobileAssociationBonusOutput;
        object MobileCareersJobAdOutput;
        object MobileBannerAdOutput;
        object MobileUpdateNoticeOutput;
        object FlagOptionOutput;
        object InboxItemOutput;
        object InfoOutput;
        object NetworkUserOutput;
        object NotificationOutput;
        object PostOutput;
        object PrivilegeOutput;
        object QuestionOutput;
        object QuestionTimelineOutput;
        object ReputationOutput;
        object ReputationHistoryOutput;
        object RevisionOutput;
        object SearchExcerptOutput;
        object ShallowUserOutput;
        object SuggestedEditOutput;
        object TagOutput;
        object TagScoreOutput;
        object TagSynonymOutput;
        object TagWikiOutput;
        object TopTagOutput;
        object UserOutput;
        object UserTimelineOutput;
        object WritePermissionOutput;
        object MobileBannerAdImageOutput;
        object SiteOutput;
        object RelatedSiteOutput;
        object ClosedDetailsOutput;
        object NoticeOutput;
        object MigrationInfoOutput;
        object BadgeCountOutput;
        object StylingOutput;
        object OriginalQuestionOutput;

        [GlobalSetup]
        public void Setup()
        {
            // primitives
            SByteOutput = Serializer.Serialize(SByteInput);
            ShortOutput = Serializer.Serialize(ShortInput);
            IntOutput = Serializer.Serialize(IntInput);
            LongOutput = Serializer.Serialize(LongInput);
            ByteOutput = Serializer.Serialize(ByteInput);
            UShortOutput = Serializer.Serialize(UShortInput);
            UIntOutput = Serializer.Serialize(UIntInput);
            ULongOutput = Serializer.Serialize(ULongInput);
            BoolOutput = Serializer.Serialize(BoolInput);
            StringOutput = Serializer.Serialize(StringInput);
            CharOutput = Serializer.Serialize(CharInput);
            DateTimeOutput = Serializer.Serialize(DateTimeInput);
            GuidOutput = Serializer.Serialize(GuidInput);
            BytesOutput = Serializer.Serialize(BytesInput);

            // models
            AccessTokenOutput = Serializer.Serialize(AccessTokenInput);
            AccountMergeOutput = Serializer.Serialize(AccountMergeInput);
            AnswerOutput = Serializer.Serialize(AnswerInput);
            BadgeOutput = Serializer.Serialize(BadgeInput);
            CommentOutput = Serializer.Serialize(CommentInput);
            ErrorOutput = Serializer.Serialize(ErrorInput);
            EventOutput = Serializer.Serialize(EventInput);
            MobileFeedOutput = Serializer.Serialize(MobileFeedInput);
            MobileQuestionOutput = Serializer.Serialize(MobileQuestionInput);
            MobileRepChangeOutput = Serializer.Serialize(MobileRepChangeInput);
            MobileInboxItemOutput = Serializer.Serialize(MobileInboxItemInput);
            MobileBadgeAwardOutput = Serializer.Serialize(MobileBadgeAwardInput);
            MobilePrivilegeOutput = Serializer.Serialize(MobilePrivilegeInput);
            MobileCommunityBulletinOutput = Serializer.Serialize(MobileCommunityBulletinInput);
            MobileAssociationBonusOutput = Serializer.Serialize(MobileAssociationBonusInput);
            MobileCareersJobAdOutput = Serializer.Serialize(MobileCareersJobAdInput);
            MobileBannerAdOutput = Serializer.Serialize(MobileBannerAdInput);
            MobileUpdateNoticeOutput = Serializer.Serialize(MobileUpdateNoticeInput);
            FlagOptionOutput = Serializer.Serialize(FlagOptionInput);
            InboxItemOutput = Serializer.Serialize(InboxItemInput);
            InfoOutput = Serializer.Serialize(InfoInput);
            NetworkUserOutput = Serializer.Serialize(NetworkUserInput);
            NotificationOutput = Serializer.Serialize(NotificationInput);
            PostOutput = Serializer.Serialize(PostInput);
            PrivilegeOutput = Serializer.Serialize(PrivilegeInput);
            QuestionOutput = Serializer.Serialize(QuestionInput);
            QuestionTimelineOutput = Serializer.Serialize(QuestionTimelineInput);
            ReputationOutput = Serializer.Serialize(ReputationInput);
            ReputationHistoryOutput = Serializer.Serialize(ReputationHistoryInput);
            RevisionOutput = Serializer.Serialize(RevisionInput);
            SearchExcerptOutput = Serializer.Serialize(SearchExcerptInput);
            ShallowUserOutput = Serializer.Serialize(ShallowUserInput);
            SuggestedEditOutput = Serializer.Serialize(SuggestedEditInput);
            TagOutput = Serializer.Serialize(TagInput);
            TagScoreOutput = Serializer.Serialize(TagScoreInput);
            TagSynonymOutput = Serializer.Serialize(TagSynonymInput);
            TagWikiOutput = Serializer.Serialize(TagWikiInput);
            TopTagOutput = Serializer.Serialize(TopTagInput);
            UserOutput = Serializer.Serialize(UserInput);
            UserTimelineOutput = Serializer.Serialize(UserTimelineInput);
            WritePermissionOutput = Serializer.Serialize(WritePermissionInput);
            MobileBannerAdImageOutput = Serializer.Serialize(MobileBannerAdImageInput);
            SiteOutput = Serializer.Serialize(SiteInput);
            RelatedSiteOutput = Serializer.Serialize(RelatedSiteInput);
            ClosedDetailsOutput = Serializer.Serialize(ClosedDetailsInput);
            NoticeOutput = Serializer.Serialize(NoticeInput);
            MigrationInfoOutput = Serializer.Serialize(MigrationInfoInput);
            BadgeCountOutput = Serializer.Serialize(BadgeCountInput);
            StylingOutput = Serializer.Serialize(StylingInput);
            OriginalQuestionOutput = Serializer.Serialize(OriginalQuestionInput);
        }

        // Serialize

        [Benchmark] public object _PrimitiveSByteSerialize() => Serializer.Serialize(SByteInput);
        [Benchmark] public object _PrimitiveShortSerialize() => Serializer.Serialize(ShortInput);
        [Benchmark] public object _PrimitiveIntSerialize() => Serializer.Serialize(IntInput);
        [Benchmark] public object _PrimitiveLongSerialize() => Serializer.Serialize(LongInput);
        [Benchmark] public object _PrimitiveByteSerialize() => Serializer.Serialize(ByteInput);
        [Benchmark] public object _PrimitiveUShortSerialize() => Serializer.Serialize(UShortInput);
        [Benchmark] public object _PrimitiveUIntSerialize() => Serializer.Serialize(UIntInput);
        [Benchmark] public object _PrimitiveULongSerialize() => Serializer.Serialize(ULongInput);
        [Benchmark] public object _PrimitiveBoolSerialize() => Serializer.Serialize(BoolInput);
        [Benchmark] public object _PrimitiveStringSerialize() => Serializer.Serialize(StringInput);
        [Benchmark] public object _PrimitiveCharSerialize() => Serializer.Serialize(CharInput);
        [Benchmark] public object _PrimitiveDateTimeSerialize() => Serializer.Serialize(DateTimeInput);
        [Benchmark] public object _PrimitiveGuidSerialize() => Serializer.Serialize(GuidInput);
        [Benchmark] public object _PrimitiveBytesSerialize() => Serializer.Serialize(BytesInput);

        [Benchmark] public object AccessTokenSerialize() => Serializer.Serialize(AccessTokenInput);
        [Benchmark] public object AccountMergeSerialize() => Serializer.Serialize(AccountMergeInput);
        [Benchmark] public object AnswerSerialize() => Serializer.Serialize(AnswerInput);
        [Benchmark] public object BadgeSerialize() => Serializer.Serialize(BadgeInput);
        [Benchmark] public object CommentSerialize() => Serializer.Serialize(CommentInput);
        [Benchmark] public object ErrorSerialize() => Serializer.Serialize(ErrorInput);
        [Benchmark] public object EventSerialize() => Serializer.Serialize(EventInput);
        [Benchmark] public object MobileFeedSerialize() => Serializer.Serialize(MobileFeedInput);
        [Benchmark] public object MobileQuestionSerialize() => Serializer.Serialize(MobileQuestionInput);
        [Benchmark] public object MobileRepChangeSerialize() => Serializer.Serialize(MobileRepChangeInput);
        [Benchmark] public object MobileInboxItemSerialize() => Serializer.Serialize(MobileInboxItemInput);
        [Benchmark] public object MobileBadgeAwardSerialize() => Serializer.Serialize(MobileBadgeAwardInput);
        [Benchmark] public object MobilePrivilegeSerialize() => Serializer.Serialize(MobilePrivilegeInput);
        [Benchmark] public object MobileCommunityBulletinSerialize() => Serializer.Serialize(MobileCommunityBulletinInput);
        [Benchmark] public object MobileAssociationBonusSerialize() => Serializer.Serialize(MobileAssociationBonusInput);
        [Benchmark] public object MobileCareersJobAdSerialize() => Serializer.Serialize(MobileCareersJobAdInput);
        [Benchmark] public object MobileBannerAdSerialize() => Serializer.Serialize(MobileBannerAdInput);
        [Benchmark] public object MobileUpdateNoticeSerialize() => Serializer.Serialize(MobileUpdateNoticeInput);
        [Benchmark] public object FlagOptionSerialize() => Serializer.Serialize(FlagOptionInput);
        [Benchmark] public object InboxItemSerialize() => Serializer.Serialize(InboxItemInput);
        [Benchmark] public object InfoSerialize() => Serializer.Serialize(InfoInput);
        [Benchmark] public object NetworkUserSerialize() => Serializer.Serialize(NetworkUserInput);
        [Benchmark] public object NotificationSerialize() => Serializer.Serialize(NotificationInput);
        [Benchmark] public object PostSerialize() => Serializer.Serialize(PostInput);
        [Benchmark] public object PrivilegeSerialize() => Serializer.Serialize(PrivilegeInput);
        [Benchmark] public object QuestionSerialize() => Serializer.Serialize(QuestionInput);
        [Benchmark] public object QuestionTimelineSerialize() => Serializer.Serialize(QuestionTimelineInput);
        [Benchmark] public object ReputationSerialize() => Serializer.Serialize(ReputationInput);
        [Benchmark] public object ReputationHistorySerialize() => Serializer.Serialize(ReputationHistoryInput);
        [Benchmark] public object RevisionSerialize() => Serializer.Serialize(RevisionInput);
        [Benchmark] public object SearchExcerptSerialize() => Serializer.Serialize(SearchExcerptInput);
        [Benchmark] public object ShallowUserSerialize() => Serializer.Serialize(ShallowUserInput);
        [Benchmark] public object SuggestedEditSerialize() => Serializer.Serialize(SuggestedEditInput);
        [Benchmark] public object TagSerialize() => Serializer.Serialize(TagInput);
        [Benchmark] public object TagScoreSerialize() => Serializer.Serialize(TagScoreInput);
        [Benchmark] public object TagSynonymSerialize() => Serializer.Serialize(TagSynonymInput);
        [Benchmark] public object TagWikiSerialize() => Serializer.Serialize(TagWikiInput);
        [Benchmark] public object TopTagSerialize() => Serializer.Serialize(TopTagInput);
        [Benchmark] public object UserSerialize() => Serializer.Serialize(UserInput);
        [Benchmark] public object UserTimelineSerialize() => Serializer.Serialize(UserTimelineInput);
        [Benchmark] public object WritePermissionSerialize() => Serializer.Serialize(WritePermissionInput);
        [Benchmark] public object MobileBannerAdImageSerialize() => Serializer.Serialize(MobileBannerAdImageInput);
        [Benchmark] public object SiteSerialize() => Serializer.Serialize(SiteInput);
        [Benchmark] public object RelatedSiteSerialize() => Serializer.Serialize(RelatedSiteInput);
        [Benchmark] public object ClosedDetailsSerialize() => Serializer.Serialize(ClosedDetailsInput);
        [Benchmark] public object NoticeSerialize() => Serializer.Serialize(NoticeInput);
        [Benchmark] public object MigrationInfoSerialize() => Serializer.Serialize(MigrationInfoInput);
        [Benchmark] public object BadgeCountSerialize() => Serializer.Serialize(BadgeCountInput);
        [Benchmark] public object StylingSerialize() => Serializer.Serialize(StylingInput);
        [Benchmark] public object OriginalQuestionSerialize() => Serializer.Serialize(OriginalQuestionInput);

        // Deserialize

        [Benchmark] public SByte _PrimitiveSByteDeserialize() => Serializer.Deserialize<SByte>(SByteOutput);
        [Benchmark] public short _PrimitiveShortDeserialize() => Serializer.Deserialize<short>(ShortOutput);
        [Benchmark] public Int32 _PrimitiveIntDeserialize() => Serializer.Deserialize<Int32>(IntOutput);
        [Benchmark] public Int64 _PrimitiveLongDeserialize() => Serializer.Deserialize<Int64>(LongOutput);
        [Benchmark] public Byte _PrimitiveByteDeserialize() => Serializer.Deserialize<Byte>(ByteOutput);
        [Benchmark] public ushort _PrimitiveUShortDeserialize() => Serializer.Deserialize<ushort>(UShortOutput);
        [Benchmark] public uint _PrimitiveUIntDeserialize() => Serializer.Deserialize<uint>(UIntOutput);
        [Benchmark] public ulong _PrimitiveULongDeserialize() => Serializer.Deserialize<ulong>(ULongOutput);
        [Benchmark] public bool _PrimitiveBoolDeserialize() => Serializer.Deserialize<bool>(BoolOutput);
        [Benchmark] public String _PrimitiveStringDeserialize() => Serializer.Deserialize<String>(StringOutput);
        [Benchmark] public Char _PrimitiveCharDeserialize() => Serializer.Deserialize<Char>(CharOutput);
        [Benchmark] public DateTime _PrimitiveDateTimeDeserialize() => Serializer.Deserialize<DateTime>(DateTimeOutput);
        [Benchmark] public Guid _PrimitiveGuidDeserialize() => Serializer.Deserialize<Guid>(GuidOutput);
        [Benchmark] public byte[] _PrimitiveBytesDeserialize() => Serializer.Deserialize<byte[]>(BytesOutput);
        [Benchmark] public AccessToken AccessTokenDeserialize() => Serializer.Deserialize<AccessToken>(AccessTokenOutput);
        [Benchmark] public AccountMerge AccountMergeDeserialize() => Serializer.Deserialize<AccountMerge>(AccountMergeOutput);
        [Benchmark] public Answer AnswerDeserialize() => Serializer.Deserialize<Answer>(AnswerOutput);
        [Benchmark] public Badge BadgeDeserialize() => Serializer.Deserialize<Badge>(BadgeOutput);
        [Benchmark] public Comment CommentDeserialize() => Serializer.Deserialize<Comment>(CommentOutput);
        [Benchmark] public Error ErrorDeserialize() => Serializer.Deserialize<Error>(ErrorOutput);
        [Benchmark] public Event EventDeserialize() => Serializer.Deserialize<Event>(EventOutput);
        [Benchmark] public MobileFeed MobileFeedDeserialize() => Serializer.Deserialize<MobileFeed>(MobileFeedOutput);
        [Benchmark] public MobileQuestion MobileQuestionDeserialize() => Serializer.Deserialize<MobileQuestion>(MobileQuestionOutput);
        [Benchmark] public MobileRepChange MobileRepChangeDeserialize() => Serializer.Deserialize<MobileRepChange>(MobileRepChangeOutput);
        [Benchmark] public MobileInboxItem MobileInboxItemDeserialize() => Serializer.Deserialize<MobileInboxItem>(MobileInboxItemOutput);
        [Benchmark] public MobileBadgeAward MobileBadgeAwardDeserialize() => Serializer.Deserialize<MobileBadgeAward>(MobileBadgeAwardOutput);
        [Benchmark] public MobilePrivilege MobilePrivilegeDeserialize() => Serializer.Deserialize<MobilePrivilege>(MobilePrivilegeOutput);
        [Benchmark] public MobileCommunityBulletin MobileCommunityBulletinDeserialize() => Serializer.Deserialize<MobileCommunityBulletin>(MobileCommunityBulletinOutput);
        [Benchmark] public MobileAssociationBonus MobileAssociationBonusDeserialize() => Serializer.Deserialize<MobileAssociationBonus>(MobileAssociationBonusOutput);
        [Benchmark] public MobileCareersJobAd MobileCareersJobAdDeserialize() => Serializer.Deserialize<MobileCareersJobAd>(MobileCareersJobAdOutput);
        [Benchmark] public MobileBannerAd MobileBannerAdDeserialize() => Serializer.Deserialize<MobileBannerAd>(MobileBannerAdOutput);
        [Benchmark] public MobileUpdateNotice MobileUpdateNoticeDeserialize() => Serializer.Deserialize<MobileUpdateNotice>(MobileUpdateNoticeOutput);
        [Benchmark] public FlagOption FlagOptionDeserialize() => Serializer.Deserialize<FlagOption>(FlagOptionOutput);
        [Benchmark] public InboxItem InboxItemDeserialize() => Serializer.Deserialize<InboxItem>(InboxItemOutput);
        [Benchmark] public Info InfoDeserialize() => Serializer.Deserialize<Info>(InfoOutput);
        [Benchmark] public NetworkUser NetworkUserDeserialize() => Serializer.Deserialize<NetworkUser>(NetworkUserOutput);
        [Benchmark] public Notification NotificationDeserialize() => Serializer.Deserialize<Notification>(NotificationOutput);
        [Benchmark] public Post PostDeserialize() => Serializer.Deserialize<Post>(PostOutput);
        [Benchmark] public Privilege PrivilegeDeserialize() => Serializer.Deserialize<Privilege>(PrivilegeOutput);
        [Benchmark] public Question QuestionDeserialize() => Serializer.Deserialize<Question>(QuestionOutput);
        [Benchmark] public QuestionTimeline QuestionTimelineDeserialize() => Serializer.Deserialize<QuestionTimeline>(QuestionTimelineOutput);
        [Benchmark] public Reputation ReputationDeserialize() => Serializer.Deserialize<Reputation>(ReputationOutput);
        [Benchmark] public ReputationHistory ReputationHistoryDeserialize() => Serializer.Deserialize<ReputationHistory>(ReputationHistoryOutput);
        [Benchmark] public Revision RevisionDeserialize() => Serializer.Deserialize<Revision>(RevisionOutput);
        [Benchmark] public SearchExcerpt SearchExcerptDeserialize() => Serializer.Deserialize<SearchExcerpt>(SearchExcerptOutput);
        [Benchmark] public ShallowUser ShallowUserDeserialize() => Serializer.Deserialize<ShallowUser>(ShallowUserOutput);
        [Benchmark] public SuggestedEdit SuggestedEditDeserialize() => Serializer.Deserialize<SuggestedEdit>(SuggestedEditOutput);
        [Benchmark] public Tag TagDeserialize() => Serializer.Deserialize<Tag>(TagOutput);
        [Benchmark] public TagScore TagScoreDeserialize() => Serializer.Deserialize<TagScore>(TagScoreOutput);
        [Benchmark] public TagSynonym TagSynonymDeserialize() => Serializer.Deserialize<TagSynonym>(TagSynonymOutput);
        [Benchmark] public TagWiki TagWikiDeserialize() => Serializer.Deserialize<TagWiki>(TagWikiOutput);
        [Benchmark] public TopTag TopTagDeserialize() => Serializer.Deserialize<TopTag>(TopTagOutput);
        [Benchmark] public User UserDeserialize() => Serializer.Deserialize<User>(UserOutput);
        [Benchmark] public UserTimeline UserTimelineDeserialize() => Serializer.Deserialize<UserTimeline>(UserTimelineOutput);
        [Benchmark] public WritePermission WritePermissionDeserialize() => Serializer.Deserialize<WritePermission>(WritePermissionOutput);
        [Benchmark] public MobileBannerAd.MobileBannerAdImage MobileBannerAdImageDeserialize() => Serializer.Deserialize<MobileBannerAd.MobileBannerAdImage>(MobileBannerAdImageOutput);
        [Benchmark] public Info.Site SiteDeserialize() => Serializer.Deserialize<Info.Site>(SiteOutput);
        [Benchmark] public Info.RelatedSite RelatedSiteDeserialize() => Serializer.Deserialize<Info.RelatedSite>(RelatedSiteOutput);
        [Benchmark] public Question.ClosedDetails ClosedDetailsDeserialize() => Serializer.Deserialize<Question.ClosedDetails>(ClosedDetailsOutput);
        [Benchmark] public Question.Notice NoticeDeserialize() => Serializer.Deserialize<Question.Notice>(NoticeOutput);
        [Benchmark] public Question.MigrationInfo MigrationInfoDeserialize() => Serializer.Deserialize<Question.MigrationInfo>(MigrationInfoOutput);
        [Benchmark] public User.BadgeCount BadgeCountDeserialize() => Serializer.Deserialize<User.BadgeCount>(BadgeCountOutput);
        [Benchmark] public Info.Site.Styling StylingDeserialize() => Serializer.Deserialize<Info.Site.Styling>(StylingOutput);
        [Benchmark] public Question.ClosedDetails.OriginalQuestion OriginalQuestionDeserialize() => Serializer.Deserialize<Question.ClosedDetails.OriginalQuestion>(OriginalQuestionOutput);
    }


    [Config(typeof(BenchmarkConfig))]
    public class MsgPackOfficial_Vs_MsgPackSpan_BytesInOut // : AllSerializerBenchmark
    {
        [ParamsSource(nameof(Serializers))]
        public SerializerBase Serializer;

        // Currently BenchmarkdDotNet does not detect inherited ParamsSource so use copy and paste:)

        public IEnumerable<SerializerBase> Serializers => new SerializerBase[]
        {
            new MessagePack_Official(),
            new MessagePack_Span(),
            new Typeless_Official(),
            new Typeless_Span(),
        };

        protected static readonly ExpressionTreeFixture ExpressionTreeFixture = new ExpressionTreeFixture();

        // primitives

        protected static readonly sbyte SByteInput = ExpressionTreeFixture.Create<sbyte>();
        protected static readonly short ShortInput = ExpressionTreeFixture.Create<short>();
        protected static readonly int IntInput = ExpressionTreeFixture.Create<int>();
        protected static readonly long LongInput = ExpressionTreeFixture.Create<long>();
        protected static readonly byte ByteInput = ExpressionTreeFixture.Create<byte>();
        protected static readonly ushort UShortInput = ExpressionTreeFixture.Create<ushort>();
        protected static readonly uint UIntInput = ExpressionTreeFixture.Create<uint>();
        protected static readonly ulong ULongInput = ExpressionTreeFixture.Create<ulong>();
        protected static readonly bool BoolInput = ExpressionTreeFixture.Create<bool>();
        protected static readonly string StringInput = ExpressionTreeFixture.Create<string>();
        protected static readonly char CharInput = ExpressionTreeFixture.Create<char>();
        protected static readonly DateTime DateTimeInput = ExpressionTreeFixture.Create<DateTime>();
        protected static readonly Guid GuidInput = ExpressionTreeFixture.Create<Guid>();
        protected static readonly byte[] BytesInput = ExpressionTreeFixture.Create<byte[]>();

        // models

        protected static readonly Benchmark.Models.AccessToken AccessTokenInput = ExpressionTreeFixture.Create<Benchmark.Models.AccessToken>();

        protected static readonly Benchmark.Models.AccountMerge AccountMergeInput = ExpressionTreeFixture.Create<Benchmark.Models.AccountMerge>();

        protected static readonly Benchmark.Models.Answer AnswerInput = ExpressionTreeFixture.Create<Benchmark.Models.Answer>();

        protected static readonly Benchmark.Models.Badge BadgeInput = ExpressionTreeFixture.Create<Benchmark.Models.Badge>();

        protected static readonly Benchmark.Models.Comment CommentInput = ExpressionTreeFixture.Create<Benchmark.Models.Comment>();

        protected static readonly Benchmark.Models.Error ErrorInput = ExpressionTreeFixture.Create<Benchmark.Models.Error>();

        protected static readonly Benchmark.Models.Event EventInput = ExpressionTreeFixture.Create<Benchmark.Models.Event>();

        protected static readonly Benchmark.Models.MobileFeed MobileFeedInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileFeed>();

        protected static readonly Benchmark.Models.MobileQuestion MobileQuestionInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileQuestion>();

        protected static readonly Benchmark.Models.MobileRepChange MobileRepChangeInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileRepChange>();

        protected static readonly Benchmark.Models.MobileInboxItem MobileInboxItemInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileInboxItem>();

        protected static readonly Benchmark.Models.MobileBadgeAward MobileBadgeAwardInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileBadgeAward>();

        protected static readonly Benchmark.Models.MobilePrivilege MobilePrivilegeInput = ExpressionTreeFixture.Create<Benchmark.Models.MobilePrivilege>();

        protected static readonly Benchmark.Models.MobileCommunityBulletin MobileCommunityBulletinInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileCommunityBulletin>();

        protected static readonly Benchmark.Models.MobileAssociationBonus MobileAssociationBonusInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileAssociationBonus>();

        protected static readonly Benchmark.Models.MobileCareersJobAd MobileCareersJobAdInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileCareersJobAd>();

        protected static readonly Benchmark.Models.MobileBannerAd MobileBannerAdInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileBannerAd>();

        protected static readonly Benchmark.Models.MobileUpdateNotice MobileUpdateNoticeInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileUpdateNotice>();

        protected static readonly Benchmark.Models.FlagOption FlagOptionInput = ExpressionTreeFixture.Create<Benchmark.Models.FlagOption>();

        protected static readonly Benchmark.Models.InboxItem InboxItemInput = ExpressionTreeFixture.Create<Benchmark.Models.InboxItem>();

        protected static readonly Benchmark.Models.Info InfoInput = ExpressionTreeFixture.Create<Benchmark.Models.Info>();

        protected static readonly Benchmark.Models.NetworkUser NetworkUserInput = ExpressionTreeFixture.Create<Benchmark.Models.NetworkUser>();

        protected static readonly Benchmark.Models.Notification NotificationInput = ExpressionTreeFixture.Create<Benchmark.Models.Notification>();

        protected static readonly Benchmark.Models.Post PostInput = ExpressionTreeFixture.Create<Benchmark.Models.Post>();

        protected static readonly Benchmark.Models.Privilege PrivilegeInput = ExpressionTreeFixture.Create<Benchmark.Models.Privilege>();

        protected static readonly Benchmark.Models.Question QuestionInput = ExpressionTreeFixture.Create<Benchmark.Models.Question>();

        protected static readonly Benchmark.Models.QuestionTimeline QuestionTimelineInput = ExpressionTreeFixture.Create<Benchmark.Models.QuestionTimeline>();

        protected static readonly Benchmark.Models.Reputation ReputationInput = ExpressionTreeFixture.Create<Benchmark.Models.Reputation>();

        protected static readonly Benchmark.Models.ReputationHistory ReputationHistoryInput = ExpressionTreeFixture.Create<Benchmark.Models.ReputationHistory>();

        protected static readonly Benchmark.Models.Revision RevisionInput = ExpressionTreeFixture.Create<Benchmark.Models.Revision>();

        protected static readonly Benchmark.Models.SearchExcerpt SearchExcerptInput = ExpressionTreeFixture.Create<Benchmark.Models.SearchExcerpt>();

        protected static readonly Benchmark.Models.ShallowUser ShallowUserInput = ExpressionTreeFixture.Create<Benchmark.Models.ShallowUser>();

        protected static readonly Benchmark.Models.SuggestedEdit SuggestedEditInput = ExpressionTreeFixture.Create<Benchmark.Models.SuggestedEdit>();

        protected static readonly Benchmark.Models.Tag TagInput = ExpressionTreeFixture.Create<Benchmark.Models.Tag>();

        protected static readonly Benchmark.Models.TagScore TagScoreInput = ExpressionTreeFixture.Create<Benchmark.Models.TagScore>();

        protected static readonly Benchmark.Models.TagSynonym TagSynonymInput = ExpressionTreeFixture.Create<Benchmark.Models.TagSynonym>();

        protected static readonly Benchmark.Models.TagWiki TagWikiInput = ExpressionTreeFixture.Create<Benchmark.Models.TagWiki>();

        protected static readonly Benchmark.Models.TopTag TopTagInput = ExpressionTreeFixture.Create<Benchmark.Models.TopTag>();

        protected static readonly Benchmark.Models.User UserInput = ExpressionTreeFixture.Create<Benchmark.Models.User>();

        protected static readonly Benchmark.Models.UserTimeline UserTimelineInput = ExpressionTreeFixture.Create<Benchmark.Models.UserTimeline>();

        protected static readonly Benchmark.Models.WritePermission WritePermissionInput = ExpressionTreeFixture.Create<Benchmark.Models.WritePermission>();

        protected static readonly Benchmark.Models.MobileBannerAd.MobileBannerAdImage MobileBannerAdImageInput = ExpressionTreeFixture.Create<Benchmark.Models.MobileBannerAd.MobileBannerAdImage>();

        protected static readonly Benchmark.Models.Info.Site SiteInput = ExpressionTreeFixture.Create<Benchmark.Models.Info.Site>();

        protected static readonly Benchmark.Models.Info.RelatedSite RelatedSiteInput = ExpressionTreeFixture.Create<Benchmark.Models.Info.RelatedSite>();

        protected static readonly Benchmark.Models.Question.ClosedDetails ClosedDetailsInput = ExpressionTreeFixture.Create<Benchmark.Models.Question.ClosedDetails>();

        protected static readonly Benchmark.Models.Question.Notice NoticeInput = ExpressionTreeFixture.Create<Benchmark.Models.Question.Notice>();

        protected static readonly Benchmark.Models.Question.MigrationInfo MigrationInfoInput = ExpressionTreeFixture.Create<Benchmark.Models.Question.MigrationInfo>();

        protected static readonly Benchmark.Models.User.BadgeCount BadgeCountInput = ExpressionTreeFixture.Create<Benchmark.Models.User.BadgeCount>();

        protected static readonly Benchmark.Models.Info.Site.Styling StylingInput = ExpressionTreeFixture.Create<Benchmark.Models.Info.Site.Styling>();

        protected static readonly Benchmark.Models.Question.ClosedDetails.OriginalQuestion OriginalQuestionInput = ExpressionTreeFixture.Create<Benchmark.Models.Question.ClosedDetails.OriginalQuestion>();

        object SByteOutput;
        object ShortOutput;
        object IntOutput;
        object LongOutput;
        object ByteOutput;
        object UShortOutput;
        object UIntOutput;
        object ULongOutput;
        object BoolOutput;
        object StringOutput;
        object CharOutput;
        object DateTimeOutput;
        object GuidOutput;
        object BytesOutput;

        object AccessTokenOutput;
        object AccountMergeOutput;
        object AnswerOutput;
        object BadgeOutput;
        object CommentOutput;
        object ErrorOutput;
        object EventOutput;
        object MobileFeedOutput;
        object MobileQuestionOutput;
        object MobileRepChangeOutput;
        object MobileInboxItemOutput;
        object MobileBadgeAwardOutput;
        object MobilePrivilegeOutput;
        object MobileCommunityBulletinOutput;
        object MobileAssociationBonusOutput;
        object MobileCareersJobAdOutput;
        object MobileBannerAdOutput;
        object MobileUpdateNoticeOutput;
        object FlagOptionOutput;
        object InboxItemOutput;
        object InfoOutput;
        object NetworkUserOutput;
        object NotificationOutput;
        object PostOutput;
        object PrivilegeOutput;
        object QuestionOutput;
        object QuestionTimelineOutput;
        object ReputationOutput;
        object ReputationHistoryOutput;
        object RevisionOutput;
        object SearchExcerptOutput;
        object ShallowUserOutput;
        object SuggestedEditOutput;
        object TagOutput;
        object TagScoreOutput;
        object TagSynonymOutput;
        object TagWikiOutput;
        object TopTagOutput;
        object UserOutput;
        object UserTimelineOutput;
        object WritePermissionOutput;
        object MobileBannerAdImageOutput;
        object SiteOutput;
        object RelatedSiteOutput;
        object ClosedDetailsOutput;
        object NoticeOutput;
        object MigrationInfoOutput;
        object BadgeCountOutput;
        object StylingOutput;
        object OriginalQuestionOutput;

        [GlobalSetup]
        public void Setup()
        {
            // primitives
            SByteOutput = Serializer.Serialize(SByteInput);
            ShortOutput = Serializer.Serialize(ShortInput);
            IntOutput = Serializer.Serialize(IntInput);
            LongOutput = Serializer.Serialize(LongInput);
            ByteOutput = Serializer.Serialize(ByteInput);
            UShortOutput = Serializer.Serialize(UShortInput);
            UIntOutput = Serializer.Serialize(UIntInput);
            ULongOutput = Serializer.Serialize(ULongInput);
            BoolOutput = Serializer.Serialize(BoolInput);
            StringOutput = Serializer.Serialize(StringInput);
            CharOutput = Serializer.Serialize(CharInput);
            DateTimeOutput = Serializer.Serialize(DateTimeInput);
            GuidOutput = Serializer.Serialize(GuidInput);
            BytesOutput = Serializer.Serialize(BytesInput);

            // models
            AccessTokenOutput = Serializer.Serialize(AccessTokenInput);
            AccountMergeOutput = Serializer.Serialize(AccountMergeInput);
            AnswerOutput = Serializer.Serialize(AnswerInput);
            BadgeOutput = Serializer.Serialize(BadgeInput);
            CommentOutput = Serializer.Serialize(CommentInput);
            ErrorOutput = Serializer.Serialize(ErrorInput);
            EventOutput = Serializer.Serialize(EventInput);
            MobileFeedOutput = Serializer.Serialize(MobileFeedInput);
            MobileQuestionOutput = Serializer.Serialize(MobileQuestionInput);
            MobileRepChangeOutput = Serializer.Serialize(MobileRepChangeInput);
            MobileInboxItemOutput = Serializer.Serialize(MobileInboxItemInput);
            MobileBadgeAwardOutput = Serializer.Serialize(MobileBadgeAwardInput);
            MobilePrivilegeOutput = Serializer.Serialize(MobilePrivilegeInput);
            MobileCommunityBulletinOutput = Serializer.Serialize(MobileCommunityBulletinInput);
            MobileAssociationBonusOutput = Serializer.Serialize(MobileAssociationBonusInput);
            MobileCareersJobAdOutput = Serializer.Serialize(MobileCareersJobAdInput);
            MobileBannerAdOutput = Serializer.Serialize(MobileBannerAdInput);
            MobileUpdateNoticeOutput = Serializer.Serialize(MobileUpdateNoticeInput);
            FlagOptionOutput = Serializer.Serialize(FlagOptionInput);
            InboxItemOutput = Serializer.Serialize(InboxItemInput);
            InfoOutput = Serializer.Serialize(InfoInput);
            NetworkUserOutput = Serializer.Serialize(NetworkUserInput);
            NotificationOutput = Serializer.Serialize(NotificationInput);
            PostOutput = Serializer.Serialize(PostInput);
            PrivilegeOutput = Serializer.Serialize(PrivilegeInput);
            QuestionOutput = Serializer.Serialize(QuestionInput);
            QuestionTimelineOutput = Serializer.Serialize(QuestionTimelineInput);
            ReputationOutput = Serializer.Serialize(ReputationInput);
            ReputationHistoryOutput = Serializer.Serialize(ReputationHistoryInput);
            RevisionOutput = Serializer.Serialize(RevisionInput);
            SearchExcerptOutput = Serializer.Serialize(SearchExcerptInput);
            ShallowUserOutput = Serializer.Serialize(ShallowUserInput);
            SuggestedEditOutput = Serializer.Serialize(SuggestedEditInput);
            TagOutput = Serializer.Serialize(TagInput);
            TagScoreOutput = Serializer.Serialize(TagScoreInput);
            TagSynonymOutput = Serializer.Serialize(TagSynonymInput);
            TagWikiOutput = Serializer.Serialize(TagWikiInput);
            TopTagOutput = Serializer.Serialize(TopTagInput);
            UserOutput = Serializer.Serialize(UserInput);
            UserTimelineOutput = Serializer.Serialize(UserTimelineInput);
            WritePermissionOutput = Serializer.Serialize(WritePermissionInput);
            MobileBannerAdImageOutput = Serializer.Serialize(MobileBannerAdImageInput);
            SiteOutput = Serializer.Serialize(SiteInput);
            RelatedSiteOutput = Serializer.Serialize(RelatedSiteInput);
            ClosedDetailsOutput = Serializer.Serialize(ClosedDetailsInput);
            NoticeOutput = Serializer.Serialize(NoticeInput);
            MigrationInfoOutput = Serializer.Serialize(MigrationInfoInput);
            BadgeCountOutput = Serializer.Serialize(BadgeCountInput);
            StylingOutput = Serializer.Serialize(StylingInput);
            OriginalQuestionOutput = Serializer.Serialize(OriginalQuestionInput);
        }

        // Serialize

        [Benchmark] public object _PrimitiveSByteSerialize() => Serializer.Serialize(SByteInput);
        [Benchmark] public object _PrimitiveShortSerialize() => Serializer.Serialize(ShortInput);
        [Benchmark] public object _PrimitiveIntSerialize() => Serializer.Serialize(IntInput);
        [Benchmark] public object _PrimitiveLongSerialize() => Serializer.Serialize(LongInput);
        [Benchmark] public object _PrimitiveByteSerialize() => Serializer.Serialize(ByteInput);
        [Benchmark] public object _PrimitiveUShortSerialize() => Serializer.Serialize(UShortInput);
        [Benchmark] public object _PrimitiveUIntSerialize() => Serializer.Serialize(UIntInput);
        [Benchmark] public object _PrimitiveULongSerialize() => Serializer.Serialize(ULongInput);
        [Benchmark] public object _PrimitiveBoolSerialize() => Serializer.Serialize(BoolInput);
        [Benchmark] public object _PrimitiveStringSerialize() => Serializer.Serialize(StringInput);
        [Benchmark] public object _PrimitiveCharSerialize() => Serializer.Serialize(CharInput);
        [Benchmark] public object _PrimitiveDateTimeSerialize() => Serializer.Serialize(DateTimeInput);
        [Benchmark] public object _PrimitiveGuidSerialize() => Serializer.Serialize(GuidInput);
        [Benchmark] public object _PrimitiveBytesSerialize() => Serializer.Serialize(BytesInput);

        [Benchmark] public object AccessTokenSerialize() => Serializer.Serialize(AccessTokenInput);
        [Benchmark] public object AccountMergeSerialize() => Serializer.Serialize(AccountMergeInput);
        [Benchmark] public object AnswerSerialize() => Serializer.Serialize(AnswerInput);
        [Benchmark] public object BadgeSerialize() => Serializer.Serialize(BadgeInput);
        [Benchmark] public object CommentSerialize() => Serializer.Serialize(CommentInput);
        [Benchmark] public object ErrorSerialize() => Serializer.Serialize(ErrorInput);
        [Benchmark] public object EventSerialize() => Serializer.Serialize(EventInput);
        [Benchmark] public object MobileFeedSerialize() => Serializer.Serialize(MobileFeedInput);
        [Benchmark] public object MobileQuestionSerialize() => Serializer.Serialize(MobileQuestionInput);
        [Benchmark] public object MobileRepChangeSerialize() => Serializer.Serialize(MobileRepChangeInput);
        [Benchmark] public object MobileInboxItemSerialize() => Serializer.Serialize(MobileInboxItemInput);
        [Benchmark] public object MobileBadgeAwardSerialize() => Serializer.Serialize(MobileBadgeAwardInput);
        [Benchmark] public object MobilePrivilegeSerialize() => Serializer.Serialize(MobilePrivilegeInput);
        [Benchmark] public object MobileCommunityBulletinSerialize() => Serializer.Serialize(MobileCommunityBulletinInput);
        [Benchmark] public object MobileAssociationBonusSerialize() => Serializer.Serialize(MobileAssociationBonusInput);
        [Benchmark] public object MobileCareersJobAdSerialize() => Serializer.Serialize(MobileCareersJobAdInput);
        [Benchmark] public object MobileBannerAdSerialize() => Serializer.Serialize(MobileBannerAdInput);
        [Benchmark] public object MobileUpdateNoticeSerialize() => Serializer.Serialize(MobileUpdateNoticeInput);
        [Benchmark] public object FlagOptionSerialize() => Serializer.Serialize(FlagOptionInput);
        [Benchmark] public object InboxItemSerialize() => Serializer.Serialize(InboxItemInput);
        [Benchmark] public object InfoSerialize() => Serializer.Serialize(InfoInput);
        [Benchmark] public object NetworkUserSerialize() => Serializer.Serialize(NetworkUserInput);
        [Benchmark] public object NotificationSerialize() => Serializer.Serialize(NotificationInput);
        [Benchmark] public object PostSerialize() => Serializer.Serialize(PostInput);
        [Benchmark] public object PrivilegeSerialize() => Serializer.Serialize(PrivilegeInput);
        [Benchmark] public object QuestionSerialize() => Serializer.Serialize(QuestionInput);
        [Benchmark] public object QuestionTimelineSerialize() => Serializer.Serialize(QuestionTimelineInput);
        [Benchmark] public object ReputationSerialize() => Serializer.Serialize(ReputationInput);
        [Benchmark] public object ReputationHistorySerialize() => Serializer.Serialize(ReputationHistoryInput);
        [Benchmark] public object RevisionSerialize() => Serializer.Serialize(RevisionInput);
        [Benchmark] public object SearchExcerptSerialize() => Serializer.Serialize(SearchExcerptInput);
        [Benchmark] public object ShallowUserSerialize() => Serializer.Serialize(ShallowUserInput);
        [Benchmark] public object SuggestedEditSerialize() => Serializer.Serialize(SuggestedEditInput);
        [Benchmark] public object TagSerialize() => Serializer.Serialize(TagInput);
        [Benchmark] public object TagScoreSerialize() => Serializer.Serialize(TagScoreInput);
        [Benchmark] public object TagSynonymSerialize() => Serializer.Serialize(TagSynonymInput);
        [Benchmark] public object TagWikiSerialize() => Serializer.Serialize(TagWikiInput);
        [Benchmark] public object TopTagSerialize() => Serializer.Serialize(TopTagInput);
        [Benchmark] public object UserSerialize() => Serializer.Serialize(UserInput);
        [Benchmark] public object UserTimelineSerialize() => Serializer.Serialize(UserTimelineInput);
        [Benchmark] public object WritePermissionSerialize() => Serializer.Serialize(WritePermissionInput);
        [Benchmark] public object MobileBannerAdImageSerialize() => Serializer.Serialize(MobileBannerAdImageInput);
        [Benchmark] public object SiteSerialize() => Serializer.Serialize(SiteInput);
        [Benchmark] public object RelatedSiteSerialize() => Serializer.Serialize(RelatedSiteInput);
        [Benchmark] public object ClosedDetailsSerialize() => Serializer.Serialize(ClosedDetailsInput);
        [Benchmark] public object NoticeSerialize() => Serializer.Serialize(NoticeInput);
        [Benchmark] public object MigrationInfoSerialize() => Serializer.Serialize(MigrationInfoInput);
        [Benchmark] public object BadgeCountSerialize() => Serializer.Serialize(BadgeCountInput);
        [Benchmark] public object StylingSerialize() => Serializer.Serialize(StylingInput);
        [Benchmark] public object OriginalQuestionSerialize() => Serializer.Serialize(OriginalQuestionInput);

        // Deserialize

        [Benchmark] public SByte _PrimitiveSByteDeserialize() => Serializer.Deserialize<SByte>(SByteOutput);
        [Benchmark] public short _PrimitiveShortDeserialize() => Serializer.Deserialize<short>(ShortOutput);
        [Benchmark] public Int32 _PrimitiveIntDeserialize() => Serializer.Deserialize<Int32>(IntOutput);
        [Benchmark] public Int64 _PrimitiveLongDeserialize() => Serializer.Deserialize<Int64>(LongOutput);
        [Benchmark] public Byte _PrimitiveByteDeserialize() => Serializer.Deserialize<Byte>(ByteOutput);
        [Benchmark] public ushort _PrimitiveUShortDeserialize() => Serializer.Deserialize<ushort>(UShortOutput);
        [Benchmark] public uint _PrimitiveUIntDeserialize() => Serializer.Deserialize<uint>(UIntOutput);
        [Benchmark] public ulong _PrimitiveULongDeserialize() => Serializer.Deserialize<ulong>(ULongOutput);
        [Benchmark] public bool _PrimitiveBoolDeserialize() => Serializer.Deserialize<bool>(BoolOutput);
        [Benchmark] public String _PrimitiveStringDeserialize() => Serializer.Deserialize<String>(StringOutput);
        [Benchmark] public Char _PrimitiveCharDeserialize() => Serializer.Deserialize<Char>(CharOutput);
        [Benchmark] public DateTime _PrimitiveDateTimeDeserialize() => Serializer.Deserialize<DateTime>(DateTimeOutput);
        [Benchmark] public Guid _PrimitiveGuidDeserialize() => Serializer.Deserialize<Guid>(GuidOutput);
        [Benchmark] public byte[] _PrimitiveBytesDeserialize() => Serializer.Deserialize<byte[]>(BytesOutput);
        [Benchmark] public AccessToken AccessTokenDeserialize() => Serializer.Deserialize<AccessToken>(AccessTokenOutput);
        [Benchmark] public AccountMerge AccountMergeDeserialize() => Serializer.Deserialize<AccountMerge>(AccountMergeOutput);
        [Benchmark] public Answer AnswerDeserialize() => Serializer.Deserialize<Answer>(AnswerOutput);
        [Benchmark] public Badge BadgeDeserialize() => Serializer.Deserialize<Badge>(BadgeOutput);
        [Benchmark] public Comment CommentDeserialize() => Serializer.Deserialize<Comment>(CommentOutput);
        [Benchmark] public Error ErrorDeserialize() => Serializer.Deserialize<Error>(ErrorOutput);
        [Benchmark] public Event EventDeserialize() => Serializer.Deserialize<Event>(EventOutput);
        [Benchmark] public MobileFeed MobileFeedDeserialize() => Serializer.Deserialize<MobileFeed>(MobileFeedOutput);
        [Benchmark] public MobileQuestion MobileQuestionDeserialize() => Serializer.Deserialize<MobileQuestion>(MobileQuestionOutput);
        [Benchmark] public MobileRepChange MobileRepChangeDeserialize() => Serializer.Deserialize<MobileRepChange>(MobileRepChangeOutput);
        [Benchmark] public MobileInboxItem MobileInboxItemDeserialize() => Serializer.Deserialize<MobileInboxItem>(MobileInboxItemOutput);
        [Benchmark] public MobileBadgeAward MobileBadgeAwardDeserialize() => Serializer.Deserialize<MobileBadgeAward>(MobileBadgeAwardOutput);
        [Benchmark] public MobilePrivilege MobilePrivilegeDeserialize() => Serializer.Deserialize<MobilePrivilege>(MobilePrivilegeOutput);
        [Benchmark] public MobileCommunityBulletin MobileCommunityBulletinDeserialize() => Serializer.Deserialize<MobileCommunityBulletin>(MobileCommunityBulletinOutput);
        [Benchmark] public MobileAssociationBonus MobileAssociationBonusDeserialize() => Serializer.Deserialize<MobileAssociationBonus>(MobileAssociationBonusOutput);
        [Benchmark] public MobileCareersJobAd MobileCareersJobAdDeserialize() => Serializer.Deserialize<MobileCareersJobAd>(MobileCareersJobAdOutput);
        [Benchmark] public MobileBannerAd MobileBannerAdDeserialize() => Serializer.Deserialize<MobileBannerAd>(MobileBannerAdOutput);
        [Benchmark] public MobileUpdateNotice MobileUpdateNoticeDeserialize() => Serializer.Deserialize<MobileUpdateNotice>(MobileUpdateNoticeOutput);
        [Benchmark] public FlagOption FlagOptionDeserialize() => Serializer.Deserialize<FlagOption>(FlagOptionOutput);
        [Benchmark] public InboxItem InboxItemDeserialize() => Serializer.Deserialize<InboxItem>(InboxItemOutput);
        [Benchmark] public Info InfoDeserialize() => Serializer.Deserialize<Info>(InfoOutput);
        [Benchmark] public NetworkUser NetworkUserDeserialize() => Serializer.Deserialize<NetworkUser>(NetworkUserOutput);
        [Benchmark] public Notification NotificationDeserialize() => Serializer.Deserialize<Notification>(NotificationOutput);
        [Benchmark] public Post PostDeserialize() => Serializer.Deserialize<Post>(PostOutput);
        [Benchmark] public Privilege PrivilegeDeserialize() => Serializer.Deserialize<Privilege>(PrivilegeOutput);
        [Benchmark] public Question QuestionDeserialize() => Serializer.Deserialize<Question>(QuestionOutput);
        [Benchmark] public QuestionTimeline QuestionTimelineDeserialize() => Serializer.Deserialize<QuestionTimeline>(QuestionTimelineOutput);
        [Benchmark] public Reputation ReputationDeserialize() => Serializer.Deserialize<Reputation>(ReputationOutput);
        [Benchmark] public ReputationHistory ReputationHistoryDeserialize() => Serializer.Deserialize<ReputationHistory>(ReputationHistoryOutput);
        [Benchmark] public Revision RevisionDeserialize() => Serializer.Deserialize<Revision>(RevisionOutput);
        [Benchmark] public SearchExcerpt SearchExcerptDeserialize() => Serializer.Deserialize<SearchExcerpt>(SearchExcerptOutput);
        [Benchmark] public ShallowUser ShallowUserDeserialize() => Serializer.Deserialize<ShallowUser>(ShallowUserOutput);
        [Benchmark] public SuggestedEdit SuggestedEditDeserialize() => Serializer.Deserialize<SuggestedEdit>(SuggestedEditOutput);
        [Benchmark] public Tag TagDeserialize() => Serializer.Deserialize<Tag>(TagOutput);
        [Benchmark] public TagScore TagScoreDeserialize() => Serializer.Deserialize<TagScore>(TagScoreOutput);
        [Benchmark] public TagSynonym TagSynonymDeserialize() => Serializer.Deserialize<TagSynonym>(TagSynonymOutput);
        [Benchmark] public TagWiki TagWikiDeserialize() => Serializer.Deserialize<TagWiki>(TagWikiOutput);
        [Benchmark] public TopTag TopTagDeserialize() => Serializer.Deserialize<TopTag>(TopTagOutput);
        [Benchmark] public User UserDeserialize() => Serializer.Deserialize<User>(UserOutput);
        [Benchmark] public UserTimeline UserTimelineDeserialize() => Serializer.Deserialize<UserTimeline>(UserTimelineOutput);
        [Benchmark] public WritePermission WritePermissionDeserialize() => Serializer.Deserialize<WritePermission>(WritePermissionOutput);
        [Benchmark] public MobileBannerAd.MobileBannerAdImage MobileBannerAdImageDeserialize() => Serializer.Deserialize<MobileBannerAd.MobileBannerAdImage>(MobileBannerAdImageOutput);
        [Benchmark] public Info.Site SiteDeserialize() => Serializer.Deserialize<Info.Site>(SiteOutput);
        [Benchmark] public Info.RelatedSite RelatedSiteDeserialize() => Serializer.Deserialize<Info.RelatedSite>(RelatedSiteOutput);
        [Benchmark] public Question.ClosedDetails ClosedDetailsDeserialize() => Serializer.Deserialize<Question.ClosedDetails>(ClosedDetailsOutput);
        [Benchmark] public Question.Notice NoticeDeserialize() => Serializer.Deserialize<Question.Notice>(NoticeOutput);
        [Benchmark] public Question.MigrationInfo MigrationInfoDeserialize() => Serializer.Deserialize<Question.MigrationInfo>(MigrationInfoOutput);
        [Benchmark] public User.BadgeCount BadgeCountDeserialize() => Serializer.Deserialize<User.BadgeCount>(BadgeCountOutput);
        [Benchmark] public Info.Site.Styling StylingDeserialize() => Serializer.Deserialize<Info.Site.Styling>(StylingOutput);
        [Benchmark] public Question.ClosedDetails.OriginalQuestion OriginalQuestionDeserialize() => Serializer.Deserialize<Question.ClosedDetails.OriginalQuestion>(OriginalQuestionOutput);
    }


    [Config(typeof(BenchmarkConfig))]
    public class ShortRun_AllSerializerBenchmark_BytesInOut
    {
        [ParamsSource(nameof(Serializers))]
        public SerializerBase Serializer;

        // Currently BenchmarkdDotNet does not detect inherited ParamsSource so use copy and paste:)

        public IEnumerable<SerializerBase> Serializers => new SerializerBase[]
        {
            new MessagePack_Official(),
            new MessagePack_Span(),
            new Typeless_Official(),
            new Typeless_Span(),
            //new MessagePackLz4_Official(),
            //new MessagePackLz4_Span(),
            new ProtobufNet(),
            //new JsonNet(),
            new BinaryFormatter_(),
            new DataContract_(),
            new Hyperion_(),
            //new Jil_(),
            new SpanJson_(),
            new Utf8Json_(),
            //new MsgPackCli(),
            //new FsPickler_(),
            //new Ceras_(),
        };

        protected static readonly ExpressionTreeFixture ExpressionTreeFixture = new ExpressionTreeFixture();

        // primitives

        protected static readonly int IntInput = ExpressionTreeFixture.Create<int>();

        // models

        protected static readonly Benchmark.Models.Answer AnswerInput = ExpressionTreeFixture.Create<Benchmark.Models.Answer>();

        object IntOutput;
        object AnswerOutput;


        [GlobalSetup]
        public void Setup()
        {
            // primitives
            IntOutput = Serializer.Serialize(IntInput);

            // models
            AnswerOutput = Serializer.Serialize(AnswerInput);
        }

        // Serialize

        [Benchmark] public object _PrimitiveIntSerialize() => Serializer.Serialize(IntInput);
        [Benchmark] public object AnswerSerialize() => Serializer.Serialize(AnswerInput);

        // Deserialize

        [Benchmark] public Int32 _PrimitiveIntDeserialize() => Serializer.Deserialize<Int32>(IntOutput);

        [Benchmark] public Answer AnswerDeserialize() => Serializer.Deserialize<Answer>(AnswerOutput);
    }

    [Config(typeof(BenchmarkConfig))]
    public class ShortRun_MsgPackOfficial_Vs_MsgPackSpan_BytesInOut
    {
        [ParamsSource(nameof(Serializers))]
        public SerializerBase Serializer;

        // Currently BenchmarkdDotNet does not detect inherited ParamsSource so use copy and paste:)

        public IEnumerable<SerializerBase> Serializers => new SerializerBase[]
        {
            new MessagePack_Official(),
            new MessagePack_Span(),
            new Typeless_Official(),
            new Typeless_Span(),
        };

        protected static readonly ExpressionTreeFixture ExpressionTreeFixture = new ExpressionTreeFixture();

        // primitives

        protected static readonly int IntInput = ExpressionTreeFixture.Create<int>();
        protected static readonly bool BoolInput = ExpressionTreeFixture.Create<bool>();
        protected static readonly string StringInput = ExpressionTreeFixture.Create<string>();
        protected static readonly Guid GuidInput = ExpressionTreeFixture.Create<Guid>();
        protected static readonly byte[] BytesInput = ExpressionTreeFixture.Create<byte[]>();

        // models

        protected static readonly Benchmark.Models.AccessToken AccessTokenInput = ExpressionTreeFixture.Create<Benchmark.Models.AccessToken>();
        protected static readonly Benchmark.Models.Error ErrorInput = ExpressionTreeFixture.Create<Benchmark.Models.Error>();
        protected static readonly Benchmark.Models.Event EventInput = ExpressionTreeFixture.Create<Benchmark.Models.Event>();
        protected static readonly Benchmark.Models.User.BadgeCount BadgeCountInput = ExpressionTreeFixture.Create<Benchmark.Models.User.BadgeCount>();

        object IntOutput;
        object BoolOutput;
        object StringOutput;
        object GuidOutput;
        object BytesOutput;

        object AccessTokenOutput;
        object ErrorOutput;
        object EventOutput;
        object BadgeCountOutput;


        [GlobalSetup]
        public void Setup()
        {
            // primitives
            IntOutput = Serializer.Serialize(IntInput);
            BoolOutput = Serializer.Serialize(BoolInput);
            StringOutput = Serializer.Serialize(StringInput);
            GuidOutput = Serializer.Serialize(GuidInput);
            BytesOutput = Serializer.Serialize(BytesInput);

            // models
            AccessTokenOutput = Serializer.Serialize(AccessTokenInput);
            ErrorOutput = Serializer.Serialize(ErrorInput);
            EventOutput = Serializer.Serialize(EventInput);
            BadgeCountOutput = Serializer.Serialize(BadgeCountInput);
        }

        // Serialize

        [Benchmark] public object _PrimitiveIntSerialize() => Serializer.Serialize(IntInput);
        [Benchmark] public object _PrimitiveBoolSerialize() => Serializer.Serialize(BoolInput);
        [Benchmark] public object _PrimitiveStringSerialize() => Serializer.Serialize(StringInput);
        [Benchmark] public object _PrimitiveGuidSerialize() => Serializer.Serialize(GuidInput);
        [Benchmark] public object _PrimitiveBytesSerialize() => Serializer.Serialize(BytesInput);

        [Benchmark] public object AccessTokenSerialize() => Serializer.Serialize(AccessTokenInput);
        [Benchmark] public object ErrorSerialize() => Serializer.Serialize(ErrorInput);
        [Benchmark] public object EventSerialize() => Serializer.Serialize(EventInput);
        [Benchmark] public object BadgeCountSerialize() => Serializer.Serialize(BadgeCountInput);

        // Deserialize

        [Benchmark] public Int32 _PrimitiveIntDeserialize() => Serializer.Deserialize<Int32>(IntOutput);
        [Benchmark] public bool _PrimitiveBoolDeserialize() => Serializer.Deserialize<bool>(BoolOutput);
        [Benchmark] public String _PrimitiveStringDeserialize() => Serializer.Deserialize<String>(StringOutput);
        [Benchmark] public Guid _PrimitiveGuidDeserialize() => Serializer.Deserialize<Guid>(GuidOutput);
        [Benchmark] public byte[] _PrimitiveBytesDeserialize() => Serializer.Deserialize<byte[]>(BytesOutput);

        [Benchmark] public AccessToken AccessTokenDeserialize() => Serializer.Deserialize<AccessToken>(AccessTokenOutput);
        [Benchmark] public Error ErrorDeserialize() => Serializer.Deserialize<Error>(ErrorOutput);
        [Benchmark] public Event EventDeserialize() => Serializer.Deserialize<Event>(EventOutput);
        [Benchmark] public User.BadgeCount BadgeCountDeserialize() => Serializer.Deserialize<User.BadgeCount>(BadgeCountOutput);
    }

    public class UnsafeMemoryBenchmark
    {
        byte v1;
        byte v2;
        byte v3;
        byte[] bytes1;
        byte[] bytes2;
        byte[] bytes3;
        byte[] srcSize4;
        byte[] srcSize8;
        byte[] srcSize16;
        byte[] srcSize32;
        byte[] srcSize64;
        byte[] dstSize4a;
        byte[] dstSize8a;
        byte[] dstSize16a;
        byte[] dstSize32a;
        byte[] dstSize64a;
        byte[] dstSize4b;
        byte[] dstSize8b;
        byte[] dstSize16b;
        byte[] dstSize32b;
        byte[] dstSize64b;

        [GlobalSetup]
        public void Setup()
        {
            v1 = 100;
            v2 = 100;
            v3 = 100;
            bytes1 = new byte[10];
            bytes2 = new byte[10];
            bytes3 = new byte[20];

            srcSize4 = new byte[4];
            Unsafe.InitBlockUnaligned(ref srcSize4[0], 4, 4);
            srcSize8 = new byte[8];
            Unsafe.InitBlockUnaligned(ref srcSize8[0], 8, 8);
            srcSize16 = new byte[16];
            Unsafe.InitBlockUnaligned(ref srcSize16[0], 16, 16);
            srcSize32 = new byte[32];
            Unsafe.InitBlockUnaligned(ref srcSize32[0], 32, 32);
            srcSize64 = new byte[64];
            Unsafe.InitBlockUnaligned(ref srcSize64[0], 64, 64);

            dstSize4a = new byte[4];
            dstSize8a = new byte[8];
            dstSize16a = new byte[16];
            dstSize32a = new byte[32];
            dstSize64a = new byte[64];

            dstSize4b = new byte[4];
            dstSize8b = new byte[8];
            dstSize16b = new byte[16];
            dstSize32b = new byte[32];
            dstSize64b = new byte[64];
        }

        [Benchmark(OperationsPerInvoke = 10000)]
        public void CopyMemorySize4()
        {
            newmsgpackcore::MessagePack.MessagePackBinary.CopyMemory(srcSize4, 0, dstSize4a, 0, 4);
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void CopyMemorySize8()
        {
            newmsgpackcore::MessagePack.MessagePackBinary.CopyMemory(srcSize8, 0, dstSize8a, 0, 8);
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void CopyMemorySize16()
        {
            newmsgpackcore::MessagePack.MessagePackBinary.CopyMemory(srcSize16, 0, dstSize16a, 0, 16);
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void CopyMemorySize32()
        {
            newmsgpackcore::MessagePack.MessagePackBinary.CopyMemory(srcSize32, 0, dstSize32a, 0, 32);
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void CopyMemorySize64()
        {
            newmsgpackcore::MessagePack.MessagePackBinary.CopyMemory(srcSize64, 0, dstSize64a, 0, 64);
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void UnsafeMemorySize4()
        {
            ref byte src = ref srcSize4[0];
            ref byte dest = ref dstSize4b[0];
            Unsafe.As<byte, int>(ref dest) = Unsafe.As<byte, int>(ref src);
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void UnsafeMemorySize8()
        {
            ref byte src = ref srcSize8[0];
            ref byte dest = ref dstSize8b[0];
            Unsafe.As<byte, long>(ref dest) = Unsafe.As<byte, long>(ref src);
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void UnsafeMemorySize16()
        {
            ref byte src = ref srcSize16[0];
            ref byte dest = ref dstSize16b[0];
            Unsafe.As<byte, long>(ref dest) = Unsafe.As<byte, long>(ref src);
            Unsafe.As<byte, long>(ref Unsafe.Add(ref dest, 8)) = Unsafe.As<byte, long>(ref Unsafe.Add(ref src, 8));
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void UnsafeMemorySize32()
        {
            ref byte src = ref srcSize32[0];
            ref byte dest = ref dstSize32b[0];
            Unsafe.As<byte, long>(ref dest) = Unsafe.As<byte, long>(ref src);
            Unsafe.As<byte, long>(ref Unsafe.Add(ref dest, 8)) = Unsafe.As<byte, long>(ref Unsafe.Add(ref src, 8));
            Unsafe.As<byte, long>(ref Unsafe.Add(ref dest, 16)) = Unsafe.As<byte, long>(ref Unsafe.Add(ref src, 16));
            Unsafe.As<byte, long>(ref Unsafe.Add(ref dest, 24)) = Unsafe.As<byte, long>(ref Unsafe.Add(ref src, 24));
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void UnsafeMemorySize64()
        {
            ref byte src = ref srcSize64[0];
            ref byte dest = ref dstSize64b[0];
            Unsafe.As<byte, long>(ref dest) = Unsafe.As<byte, long>(ref src);
            Unsafe.As<byte, long>(ref Unsafe.Add(ref dest, 8)) = Unsafe.As<byte, long>(ref Unsafe.Add(ref src, 8));
            Unsafe.As<byte, long>(ref Unsafe.Add(ref dest, 16)) = Unsafe.As<byte, long>(ref Unsafe.Add(ref src, 16));
            Unsafe.As<byte, long>(ref Unsafe.Add(ref dest, 24)) = Unsafe.As<byte, long>(ref Unsafe.Add(ref src, 24));
            Unsafe.As<byte, long>(ref Unsafe.Add(ref dest, 32)) = Unsafe.As<byte, long>(ref Unsafe.Add(ref src, 32));
            Unsafe.As<byte, long>(ref Unsafe.Add(ref dest, 40)) = Unsafe.As<byte, long>(ref Unsafe.Add(ref src, 40));
            Unsafe.As<byte, long>(ref Unsafe.Add(ref dest, 48)) = Unsafe.As<byte, long>(ref Unsafe.Add(ref src, 48));
            Unsafe.As<byte, long>(ref Unsafe.Add(ref dest, 56)) = Unsafe.As<byte, long>(ref Unsafe.Add(ref src, 56));
        }

        //[Benchmark]
        //public void Normal()
        //{
        //    for (var i = 0; i < 10; i++)
        //    {
        //        bytes1[i] = (byte)i;
        //    }
        //}

        //[Benchmark]
        //public void WriteUnsafe_WriteUnaligned()
        //{
        //    ref byte b = ref bytes2[0];
        //    IntPtr offset = (IntPtr)0;
        //    for (var i = 0; i < 10; i++)
        //    {
        //        Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref b, offset + i), (byte)i);
        //    }
        //}

        //[Benchmark]
        //public void WriteUnsafe_AddByteOffset()
        //{
        //    ref byte b = ref bytes2[0];
        //    IntPtr offset = (IntPtr)0;
        //    for (var i = 0; i < 10; i++)
        //    {
        //        Unsafe.AddByteOffset(ref b, offset + i) = (byte)i;
        //    }
        //}

        public static void WriteUInt16(ref byte[] bytes, int offset, short value)
        {
            ushort nValue = (ushort)value;
            unchecked
            {
                bytes[offset] = (byte)(nValue >> 8);
                bytes[offset + 1] = (byte)nValue;
            }
        }

        public static void WriteUInt16_Unsafe(ref byte destinationSpace, int destOffset, short value)
        {
            ushort nValue = (ushort)value;
            if (BitConverter.IsLittleEndian)
            {
                nValue = BinaryPrimitives.ReverseEndianness(nValue);
            }
            IntPtr offset = (IntPtr)destOffset;
            Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref destinationSpace, offset), nValue);
        }

        public static void WriteUInt16_Unsafe_AddByteOffset(ref byte destinationSpace, int destOffset, short value)
        {
            ushort nValue = (ushort)value;
            IntPtr offset = (IntPtr)destOffset;
            Unsafe.AddByteOffset(ref destinationSpace, offset) = (byte)(nValue >> 8);
            Unsafe.AddByteOffset(ref destinationSpace, offset + 1) = (byte)nValue;
        }

        public static void WriteUInt32(ref byte[] bytes, int offset, int value)
        {
            uint nValue = (uint)value;
            unchecked
            {
                bytes[offset] = (byte)(nValue >> 24);
                bytes[offset + 1] = (byte)(nValue >> 16);
                bytes[offset + 2] = (byte)(nValue >> 8);
                bytes[offset + 3] = (byte)nValue;
            }
        }

        public static void WriteUInt32_Unsafe(ref byte destinationSpace, int destOffset, int value)
        {
            uint nValue = (uint)value;
            if (BitConverter.IsLittleEndian)
            {
                nValue = BinaryPrimitives.ReverseEndianness(nValue);
            }
            IntPtr offset = (IntPtr)destOffset;
            Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref destinationSpace, offset), nValue);
        }

        public static void WriteUInt32_Unsafe_Add(ref byte destinationSpace, int destOffset, int value)
        {
            uint nValue = (uint)value;
            Unsafe.Add(ref destinationSpace, destOffset) = (byte)(nValue >> 24);
            Unsafe.Add(ref destinationSpace, destOffset + 1) = (byte)(nValue >> 16);
            Unsafe.Add(ref destinationSpace, destOffset + 2) = (byte)(nValue >> 8);
            Unsafe.Add(ref destinationSpace, destOffset + 3) = (byte)nValue;
        }

        public static void WriteUInt32_Unsafe_Add_Ptr(ref byte destinationSpace, int destOffset, int value)
        {
            uint nValue = (uint)value;
            IntPtr offset = (IntPtr)(uint)destOffset;
            Unsafe.Add(ref destinationSpace, offset) = (byte)(nValue >> 24);
            Unsafe.Add(ref destinationSpace, offset + 1) = (byte)(nValue >> 16);
            Unsafe.Add(ref destinationSpace, offset + 2) = (byte)(nValue >> 8);
            Unsafe.Add(ref destinationSpace, offset + 3) = (byte)nValue;
        }

        public static void WriteUInt32_Unsafe_AddByteOffset(ref byte destinationSpace, int destOffset, int value)
        {
            uint nValue = (uint)value;
            IntPtr offset = (IntPtr)destOffset;
            Unsafe.AddByteOffset(ref destinationSpace, offset) = (byte)(nValue >> 24);
            Unsafe.AddByteOffset(ref destinationSpace, offset + 1) = (byte)(nValue >> 16);
            Unsafe.AddByteOffset(ref destinationSpace, offset + 2) = (byte)(nValue >> 8);
            Unsafe.AddByteOffset(ref destinationSpace, offset + 3) = (byte)nValue;
        }
    }
}
