using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XSystem.Security.Cryptography;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ApiPeliculas.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {

        private readonly AplicationDBContext _db;
        private string claveSecreta;
        private readonly UserManager<AppUsuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UsuarioRepository(AplicationDBContext db, IConfiguration config, 
                                 RoleManager<IdentityRole> roleManager, UserManager<AppUsuario> userManager, IMapper mapper)
        {
            _db = db;
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }


        public AppUsuario GetUsuario(string usuarioId)
        {
            return _db.AppUsuario.FirstOrDefault(u => u.Id == usuarioId);
        }

        public ICollection<AppUsuario> GetUsuarios()
        {
            return _db.AppUsuario.OrderBy(u => u.UserName).ToList();
        }

        public bool IsUniqueUser(string usuario)
        {
             var usuariobd = _db.AppUsuario.FirstOrDefault(u => u.UserName==usuario);
            if (usuariobd==null) 
            {
                return true;
            }
            return false;
        }

        public async Task<UsuarioDatosDto> Registro(UsuarioRegistroDto usuarioRegistroDto)
        {
            //var passwordEncriptado = obtenermd5(usuarioRegistroDto.Password);

             AppUsuario usuario = new AppUsuario() 
             {
             UserName = usuarioRegistroDto.NombreUsuario,
             Email = usuarioRegistroDto.NombreUsuario,
             NormalizedEmail = usuarioRegistroDto.NombreUsuario.ToUpper(),
             Nombre = usuarioRegistroDto.Nombre
            

             };

            var result = await _userManager.CreateAsync(usuario, usuarioRegistroDto.Password);
            if (result.Succeeded) 
            {

                if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult()) 
                {
                    await _roleManager.CreateAsync(new IdentityRole("admin"));
                    await _roleManager.CreateAsync(new IdentityRole("registrado"));
                }
                await _userManager.AddToRoleAsync(usuario, "admin");
                var usuarioRetornado = _db.AppUsuario.FirstOrDefault(u=>u.UserName==usuarioRegistroDto.NombreUsuario);

                return _mapper.Map<UsuarioDatosDto>(usuarioRetornado);
            
            }
            //_db.Usuario.Add(usuario);
            //await _db.SaveChangesAsync();
            //usuario.Password = passwordEncriptado;
            //return usuario;

            return new UsuarioDatosDto();


        }

        public async Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto)
        {

            //var passwordEncriptado = obtenermd5(usuarioLoginDto.Password);

            var usuario = _db.AppUsuario.FirstOrDefault(
                u => u.UserName.ToLower() == usuarioLoginDto.NombreUsuario.ToLower()
            /*&& u.Password == passwordEncriptado*/);

            bool isValid = await _userManager.CheckPasswordAsync(usuario, usuarioLoginDto.Password);

            if (usuario == null || isValid == false)
            { 
                return new UsuarioLoginRespuestaDto()
                { 
                    Token = "",
                    Usuario = null
                };  
            }

            var roles= await _userManager.GetRolesAsync(usuario);

            var manejadorToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(claveSecreta);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, usuario.UserName.ToString()),
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                    }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };

            var token = manejadorToken.CreateToken(tokenDescriptor);

            UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDto()
            {
                Token = manejadorToken.WriteToken(token),
                Usuario = _mapper.Map<UsuarioDatosDto>(usuario) 
            };
            return usuarioLoginRespuestaDto;
        }




        //Método para encriptar contraseña con MD5 se usa tanto en el Acceso como en el Registro
        //public static string obtenermd5(string valor)
        //{
        //    MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
        //    byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);
        //    data = x.ComputeHash(data);
        //    string resp = "";
        //    for (int i = 0; i < data.Length; i++)
        //        resp += data[i].ToString("x2").ToLower();
        //    return resp;
        //}
    }
}
