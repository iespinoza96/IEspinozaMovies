using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace PL.Controllers
{
    public class MovieController : Controller
    {
        public ActionResult GetAll()

        {
            //TempData["Session"] = valor;
            ML.Movie pelicula = new ML.Movie();
            pelicula.Peliculas = new List<object>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://api.themoviedb.org/3/");
                var responseTask = client.GetAsync("movie/popular?api_key=67df7fa2f51e8bf444302f487e71b2e0");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode) //200
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    dynamic json = JObject.Parse(readTask.Result.ToString());

                    foreach (var res in json.results)
                    {
                        ML.Movie peliList = new ML.Movie();
                        peliList.idPelicula = res.id;
                        peliList.Portada = "https://www.themoviedb.org/t/p/w600_and_h900_bestv2" + res.poster_path;
                        peliList.Nombre = res.original_title;
                        peliList.Sinopsis = res.overview;

                        pelicula.Peliculas.Add(peliList);
                    }
                }
            }
            return View(pelicula);
        }


        public ActionResult Favorite(ML.Favorito favorito)
        {
            favorito.media_type = "movie";
            favorito.favorite = true;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
                var responseTask = client.PostAsJsonAsync<ML.Favorito>("account/15807209/favorite?api_key=67df7fa2f51e8bf444302f487e71b2e0&session_id=fa84220a4c146e7a1c964171eb9de841220192f3", favorito);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    ViewBag.Message = "Se a agregado a favoritos";
                    return View("Modal");
                }
            }
            return View("GetAll");
        }
    }
}
