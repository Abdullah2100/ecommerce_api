using api.application;
using api.domain.entity;
using data.dto.Request;
using data.dto.Response;

namespace business.mapper;

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


        private Tuple<string, int>? IsHasStore()
        {
            return (user.Store is not null) switch
            {
                true => user.Store.IsBlock
                
                    ? new Tuple<string, int>("store is Blocked", 403)
                    : null,
                _ => user.Store is null
                    ? new Tuple<string, int>("you must has store before done this operation",
                        409)
                    : null
            };
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
    extension(User? user)
    {
        public Tuple<string, int>? IsValidateFunc(bool? isAdmin = true,
            bool isStore = false)
        {
            if (user is null)
            {
                return new Tuple<string, int>("user not found", 404);
            }


            //validate user if it is admin or user according to isAdmin feild 
            switch (isAdmin)
            {
                case null: return isStore ? user.IsHasStore() : null;

                case false:
                {
                    if (user.IsBlocked)
                    {
                        return new Tuple<string, int>("user is blocked", 403);
                    }

                    //check if user has store
                    return isStore ? user.IsHasStore() : null;
                }
                default:
                {
                    if (user is { IsUser: false, IsBlocked: true })
                    {
                        return new Tuple<string, int>("user not has the permission", 403);
                    }

                    //check if admin has store
                    return isStore ? user.IsHasStore() : null;
                }
            }
        }
    }

     
}