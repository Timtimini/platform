using System.Collections.Generic;
using Otito.Services.HelperModel.Source;
using Otito.Services.HelperModel.Topic;
using Otito.Services.Model;

namespace Otito.Services
{
    public interface ITopicService
    {
        string SaveTopic(AddTopic topic, int UserId);
        int SaveClaim(AddClaim claim, int TopicId, int UserId);
        string SaveSource(string Guid, string Slug, string SlugWithoutGuid, string Source, int UserId);
        Topic GetTopic(string slug);
        TopicView TopicSimpleDetail(string slug);
        TopicView TopicEditorDetail(string slug);
        TopicView TopicEditorDetail(string slug, string search);
        TopicView ClaimDetail(int ClaimId, int TopicId);
        TopicView ClaimDetailSource(string slug);
        AddVote AddVote(int SourceId, int ClaimId, int TopicId, int Vote, int UserId);
        IList<SourceVote> GetVotes(int ClaimId);
        void DeleteTopic(string slug);
        void DeleteSource(string slug);
        void DeleteClaim(string slug);
        TopicView SourceDispute(string SourceSlug);
        void addSticky(string slug);
        void removeSticky(string slug);
        void addSlugs();
        IList<SourceVote> GetSourceVotes(IList<int> SourceIds);
    }
}