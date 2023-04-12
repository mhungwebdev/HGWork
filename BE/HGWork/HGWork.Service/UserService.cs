﻿using HGWork.DTO;
using HGWork.Model;
using HGWork.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HGWork.Service
{
    public class UserService : IUserService
    {
        private readonly HGWorkDbContext _context;
        private readonly ILogger _logger;
        public UserService(HGWorkDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResponseBase<User>> GetUserById(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            return new ResponseBase<User>()
            {
                Data = user,
                StatusCode = 200,
                Message = "Success"
            };
        }

        public async Task<ResponseBase<List<User>>> GetAll()
        {
            var res = await _context.Users.OrderByDescending(x => x.Id).ToListAsync();
            return new ResponseBase<List<User>>()
            {
                Data = res,
                StatusCode = 200,
                Message = "Success"
            };
        }

        public async Task<ResponseBase<int>> Create(User user)
        {
            if (user != null)
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return new ResponseBase<int>()
                {
                    Data = 1,
                    StatusCode = 200,
                    Message = "Success"
                };
            }
            return new ResponseBase<int>()
            {
                Data = 1,
                StatusCode = 400,
                Message = "Bad request"
            };
        }

        public async Task<ResponseBase<int>> Update(User request)
        {
            try
            {
                if (request != null)
                {
                    var project = _context.Users.FirstOrDefault(x => x.Id == request.Id);
                    if (project == null)
                    {
                        return new ResponseBase<int>
                        {
                            StatusCode = 400,
                            Data = 0,
                            Message = "Không tìm thấy dữ liệu"
                        };
                    }

                    _context.Users.Update(request);
                    await _context.SaveChangesAsync();
                    return new ResponseBase<int>
                    {
                        StatusCode = 200,
                        Data = 1,
                        Message = "Thành công"
                    };
                }
                return new ResponseBase<int>
                {
                    StatusCode = 400,
                    Data = 0,
                    Message = "Dữ liệu không đúng định dạng"
                };
            }
            catch (Exception ex)
            {
                return new ResponseBase<int>
                {
                    StatusCode = 400,
                    Data = 0,
                    Message = ex.Message
                };
            }
        }

        public async Task<ResponseBase<bool>> Login(string userName, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName.Equals(userName) && x.Password.Equals(password));
            if (user != null && user.Id > 0)
            {
                return new ResponseBase<bool>()
                {
                    Data = user.IsAdmin,
                    StatusCode = 200,
                    Message = "Success"
                };
            }

            return new ResponseBase<bool>()
            {
                Data = false,
                StatusCode = 500,
                Message = "Sai tài khoản hoặc mật khẩu"
            };
        }
    }
}