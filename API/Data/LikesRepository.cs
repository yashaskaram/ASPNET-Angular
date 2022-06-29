using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;

        public LikesRepository(DataContext context, IUserRepository user)
        {
            _context = context;
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likeParams)
        {
            var users =  _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();
            // Explanation -
            // (sourceuserid) (likeduserid) (this can be called destination)
            //          1           7
            //          1           5
            //          2           7
            //          2           6
            //          4           7

            // using table above , code below get all userids liked by userid = 1 (sourceuserid) (line 41)
            // (1, 7) AND (1, 5) (line 43)
            // second statement gets collection of userids (from line 43) liked by userid = 1 i.e., (7, 5)(line 44)
            if(likeParams.predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likeParams.userId);
                users = likes.Select(like => like.LikedUser);
            }
            // using table above, code below gets userids that liked userid = 7 (likeduserid) (line 49)
            // (1, 7), (2, 7) and (4, 7) (line 51)
            // second statement get collection of userids (from line 43) liked userid = 7 - (1, 2, 4) (line 50)
            if(likeParams.predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likeParams.userId);
                users = likes.Select(like => like.SourceUser);
            }

            var likeUsers = users.Select(user => new LikeDto 
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id 
            });

            return await PagedList<LikeDto>.CreateAsync(likeUsers, likeParams.PageNumber, likeParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                    .Include(x => x.LikedUsers)
                    .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}