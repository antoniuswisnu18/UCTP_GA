using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCTP_GA.BusinessLayer;

namespace UCTP_GA.Controllers
{
    public class GenerateScheduleController : Controller
    {
        // GET: GenerateSchedule
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult GenerateSchedule()
        {
            GeneticAlgorithm ga = new GeneticAlgorithm();
            List<Class> classes = ga.Execute();

            ViewBag.classes = classes;
            return View();
        }


    }


}