using System.Collections.Generic;
using Riok.Mapperly.Abstractions;
using ApiUserDetailModel = Flippit.Web.BL.UserDetailModel;
using ApiUserListModel = Flippit.Web.BL.UserListModel;
using ApiCardDetailModel = Flippit.Web.BL.CardDetailModel;
using ApiCardListModel = Flippit.Web.BL.CardListModel;
using ApiCollectionDetailModel = Flippit.Web.BL.CollectionDetailModel;
using ApiCollectionListModel = Flippit.Web.BL.CollectionListModel;
using ApiCompletedLessonDetailModel = Flippit.Web.BL.CompletedLessonDetailModel;
using ApiCompletedLessonListModel = Flippit.Web.BL.CompletedLessonListModel;
using ApiRole = Flippit.Web.BL.Role;
using ApiQAType = Flippit.Web.BL.QAType;

namespace Flippit.Web.BL.Mappers
{
    /// <summary>
    /// Mapper for converting between generated API client models and Common models
    /// </summary>
    [Mapper]
    public partial class ApiModelMapper
    {
        // User mappings
        public partial Flippit.Common.Models.User.UserDetailModel ToCommonUserDetail(ApiUserDetailModel apiModel);
        public partial IList<Flippit.Common.Models.User.UserDetailModel> ToCommonUserDetails(IEnumerable<ApiUserDetailModel> apiModels);
        public partial Flippit.Common.Models.User.UserListModel ToCommonUserList(ApiUserListModel apiModel);
        public partial IList<Flippit.Common.Models.User.UserListModel> ToCommonUserLists(IEnumerable<ApiUserListModel> apiModels);
        public partial ApiUserDetailModel ToApiUserDetail(Flippit.Common.Models.User.UserDetailModel commonModel);
        
        // Card mappings
        public partial Flippit.Common.Models.Card.CardDetailModel ToCommonCardDetail(ApiCardDetailModel apiModel);
        public partial IList<Flippit.Common.Models.Card.CardDetailModel> ToCommonCardDetails(IEnumerable<ApiCardDetailModel> apiModels);
        public partial Flippit.Common.Models.Card.CardListModel ToCommonCardList(ApiCardListModel apiModel);
        public partial IList<Flippit.Common.Models.Card.CardListModel> ToCommonCardLists(IEnumerable<ApiCardListModel> apiModels);
        public partial ApiCardDetailModel ToApiCardDetail(Flippit.Common.Models.Card.CardDetailModel commonModel);
        
        // Collection mappings - manual due to DateTimeOffset <-> DateTime conversion
        public Flippit.Common.Models.Collection.CollectionDetailModel ToCommonCollectionDetail(ApiCollectionDetailModel apiModel)
        {
            return new Flippit.Common.Models.Collection.CollectionDetailModel
            {
                Id = apiModel.Id,
                Name = apiModel.Name,
                CreatorId = apiModel.CreatorId,
                StartTime = apiModel.StartTime.DateTime,
                EndTime = apiModel.EndTime.DateTime
            };
        }
        
        public IList<Flippit.Common.Models.Collection.CollectionDetailModel> ToCommonCollectionDetails(IEnumerable<ApiCollectionDetailModel> apiModels)
        {
            var result = new List<Flippit.Common.Models.Collection.CollectionDetailModel>();
            foreach (var apiModel in apiModels)
            {
                result.Add(ToCommonCollectionDetail(apiModel));
            }
            return result;
        }
        
        public Flippit.Common.Models.Collection.CollectionListModel ToCommonCollectionList(ApiCollectionListModel apiModel)
        {
            return new Flippit.Common.Models.Collection.CollectionListModel
            {
                Id = apiModel.Id,
                Name = apiModel.Name,
                StartTime = apiModel.StartTime.DateTime,
                EndTime = apiModel.EndTime.DateTime
            };
        }
        
        public IList<Flippit.Common.Models.Collection.CollectionListModel> ToCommonCollectionLists(IEnumerable<ApiCollectionListModel> apiModels)
        {
            var result = new List<Flippit.Common.Models.Collection.CollectionListModel>();
            foreach (var apiModel in apiModels)
            {
                result.Add(ToCommonCollectionList(apiModel));
            }
            return result;
        }
        
        public ApiCollectionDetailModel ToApiCollectionDetail(Flippit.Common.Models.Collection.CollectionDetailModel commonModel)
        {
            return new ApiCollectionDetailModel
            {
                Id = commonModel.Id,
                Name = commonModel.Name,
                CreatorId = commonModel.CreatorId,
                StartTime = new System.DateTimeOffset(commonModel.StartTime),
                EndTime = new System.DateTimeOffset(commonModel.EndTime)
            };
        }
        
        // CompletedLesson mappings
        public partial Flippit.Common.Models.CompletedLesson.CompletedLessonDetailModel ToCommonCompletedLessonDetail(ApiCompletedLessonDetailModel apiModel);
        public partial IList<Flippit.Common.Models.CompletedLesson.CompletedLessonDetailModel> ToCommonCompletedLessonDetails(IEnumerable<ApiCompletedLessonDetailModel> apiModels);
        public partial Flippit.Common.Models.CompletedLesson.CompletedLessonListModel ToCommonCompletedLessonList(ApiCompletedLessonListModel apiModel);
        public partial IList<Flippit.Common.Models.CompletedLesson.CompletedLessonListModel> ToCommonCompletedLessonLists(IEnumerable<ApiCompletedLessonListModel> apiModels);
        public partial ApiCompletedLessonDetailModel ToApiCompletedLessonDetail(Flippit.Common.Models.CompletedLesson.CompletedLessonDetailModel commonModel);
        
        // Enum mappings
        public Flippit.Common.Enums.Role? ToCommonRole(ApiRole? apiRole)
        {
            if (apiRole == null) return null;
            return apiRole switch
            {
                ApiRole.User => Flippit.Common.Enums.Role.User,
                ApiRole.Admin => Flippit.Common.Enums.Role.Admin,
                _ => throw new System.ArgumentOutOfRangeException(nameof(apiRole), apiRole, null)
            };
        }
        
        public ApiRole? ToApiRole(Flippit.Common.Enums.Role? commonRole)
        {
            if (commonRole == null) return null;
            return commonRole switch
            {
                Flippit.Common.Enums.Role.User => ApiRole.User,
                Flippit.Common.Enums.Role.Admin => ApiRole.Admin,
                _ => throw new System.ArgumentOutOfRangeException(nameof(commonRole), commonRole, null)
            };
        }
        
        public Flippit.Common.Enums.QAType ToCommonQAType(ApiQAType apiType)
        {
            return apiType switch
            {
                ApiQAType.Text => Flippit.Common.Enums.QAType.Text,
                ApiQAType.Pictures => Flippit.Common.Enums.QAType.Pictures,
                _ => throw new System.ArgumentOutOfRangeException(nameof(apiType), apiType, null)
            };
        }
        
        public ApiQAType ToApiQAType(Flippit.Common.Enums.QAType commonType)
        {
            return commonType switch
            {
                Flippit.Common.Enums.QAType.Text => ApiQAType.Text,
                Flippit.Common.Enums.QAType.Pictures => ApiQAType.Pictures,
                _ => throw new System.ArgumentOutOfRangeException(nameof(commonType), commonType, null)
            };
        }
    }
}
