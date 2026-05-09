using api.application.Result;
using api.domain.entity;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.shared.mapper;

public static class UserMapperExtension
{
    extension(User user)
    {
        public UserInfoDto ToUserInfoDto(string url)
        {
            return new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Phone = user.Phone,
                Thumbnail = string.IsNullOrEmpty(user.Thumbnail) ? "" : url + user.Thumbnail,
                IsActive = user.IsBlocked == false,
                IsAdmin = user.IsUser == false,
                Address = user.Addresses?.Select(ad => ad.ToDto()).ToList(),
                StoreId = user?.Store?.Id,
                StoreName = user?.Store?.Name ?? "",
            };
        }

        public UserDeliveryInfoDto ToDeliveryInfoDto(string url)
        {
            return new UserDeliveryInfoDto
            {
                Email = user.Email,
                Name = user.Name,
                Phone = user.Phone,
                Thumbnail = string.IsNullOrEmpty(user.Thumbnail) ? "" : url + user.Thumbnail,
            };
        }


        private string? IsHasStore()
        {
            switch (user.Store is not null)
            {
                case true:
                {
                    if (user.Store.IsBlock)
                    {
                        return "store is Blocked";
                    }

                    return null;
                }

                default:
                {
                    if (user.Store is null)
                    {
                        return "you must has store before done this operation";
                    }

                    return null;
                }
            }
        }
    }

    extension(User? user)
    {
        public string? IsValidateFunc(bool? isAdmin = true,
            bool isStore = false)
        {
            if (user is null)
            {
                return "user not found";
            }


            //validate user if it is admin or user according to isAdmin feild 
            switch (isAdmin)
            {
                case null: return isStore ? IsHasStore(user) : null;

                case false:
                {
                    if (user.IsBlocked)
                    {
                        return "user is blocked";
                    }

                    //check if user has store
                    return isStore ? IsHasStore(user) : null;
                }
                default:
                {
                    if (user is { IsUser: false, IsBlocked: true })
                    {
                        return "user not havs the permission";
                    }

                    //check if admin has store
                    return isStore ? IsHasStore(user) : null;
                }
            }
        }
    }

    extension(UpdateUserInfoDto dto)
    {
        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(dto.Name) &&
                   string.IsNullOrWhiteSpace(dto.Phone) &&
                   dto.Thumbnail == null &&
                   string.IsNullOrWhiteSpace(dto.Password) &&
                   string.IsNullOrWhiteSpace(dto.NewPassword)
                ;
        }

        public bool IsUpdateAnyFeild()
        {
            return dto.Thumbnail != null ||
                   !(string.IsNullOrEmpty(dto.NewPassword) && string.IsNullOrEmpty(dto.Password)) ||
                   !string.IsNullOrEmpty(dto.Phone) ||
                   !string.IsNullOrEmpty(dto.Name);
        }
    }
}