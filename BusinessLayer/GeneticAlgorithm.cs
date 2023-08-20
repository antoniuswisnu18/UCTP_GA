using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;
using System.Web.UI;
using UCTP_GA.Models;

namespace UCTP_GA.BusinessLayer
{
    public class Course
    {
        public string course_id { get; set; } 
        public string course_name { get; set; }
        public int? NumberOfStudents { get; set; }
    
    }

    public class Lecturer
    {
        public string lecturer_id { get; set; }
        public string lecturer_name { get;set; }
    }

    public class Time
    {
        public string time_id { get; set; }
        public string session { get; set; }
    }

    public class Room
    {
        public string room_id { get; set; }
        public int? capacity { get; set; }
    }

    public class Class
    {
        
        public int class_id { get; set; }
        public Course course { get; set; }
        public Lecturer lecturer { get; set; }
        public Time time { get; set; }
        public Room room { get; set; }

        public Class(int classNumber)
        {
            class_id = classNumber;
        }

        public string ReturnString()
        {
            return $"{class_id}, {course.course_id}, {lecturer.lecturer_id}, {time.time_id}, {room.room_id} ";
        }
    }

    public class Schedule
    {
        public List<Class> classes = new List<Class>();
        public int numberOfClasses { get; set; }
        public int numberOfConflict { get; set; }
        public double fitnessValue = -1f;
        public bool isFitnessChanged = true;


        public void InitializeSchedule()
        {
            var dbcontext = new UCTPEntities();
            var getCourses = from course in dbcontext.Courses
                             select new Course
                             {
                                 course_id = course.CourseId,
                                 course_name = course.Name,
                                 NumberOfStudents = course.NumberOfStudents,
                             };

            var getLecturers = from lecturer in dbcontext.Lecturers
                               select new Lecturer
                               {
                                   lecturer_id = lecturer.LecturerId,
                                   lecturer_name = lecturer.Name
                               };

            var getRoom = from room in dbcontext.Rooms
                          select new Room
                          {
                              room_id = room.RoomId,
                              capacity = room.Capacity
                          };

            var getTime = from time in dbcontext.Times
                          select new Time
                          {
                              time_id = time.TimeId,
                              session = time.Session
                          };


            List<Course> courses = getCourses.ToList();
            List<Lecturer> lecturers = getLecturers.ToList();
            List<Room> rooms = getRoom.ToList();
            List<Time> times = getTime.ToList();

            

            Random random = new Random();

            foreach (Course item in courses)
            {
                numberOfClasses += 1;
                Class newClass = new Class(numberOfClasses)
                {
                    course = item,
                    lecturer = lecturers[random.Next(lecturers.Count)],
                    room = rooms[random.Next(rooms.Count)],
                    time = times[random.Next(times.Count)]
                };

                classes.Add(newClass);
                Console.WriteLine(newClass.ReturnString());
            }

        }

        public double CalculateFitness()
        {
            numberOfConflict = 0;

            for (int i = 0; i < classes.Count; i++)
            {
                if (classes[i].room.capacity < classes[i].course.NumberOfStudents)
                {
                    numberOfConflict += 1;
                }

                for (int j = 0; j < classes.Count; j++)
                {
                    if (j > i)
                    {
                        if (classes[i].time.time_id == classes[j].time.time_id && classes[i].class_id != classes[j].class_id)
                        {
                            if (classes[i].room.room_id == classes[j].room.room_id)
                            {
                                numberOfConflict += 1;
                            }

                            if (classes[i].lecturer.lecturer_id == classes[j].lecturer.lecturer_id)
                            {
                                numberOfConflict += 1;
                            }
                        }
                    }
                }
            }

            return 1/((double)numberOfConflict + 1);
        }

        public double GetFitness()
        {
            if (true)
            {
                fitnessValue = CalculateFitness();
            }

            isFitnessChanged = false;
            return fitnessValue;
        }
    }

    public class Population
    {
        public int size { get; set; }
        public List<Schedule> schedules = new List<Schedule>();

        public Population(int size)
        {
            this.size = size;
        }

        public void InitializePopulation()
        {
            for (int i = 0; i < size; i++)
            {
                Schedule newSchedule = new Schedule();
                newSchedule.InitializeSchedule();
                newSchedule.GetFitness();
                schedules.Add(newSchedule);
            }
        }
    }


    public class GeneticAlgorithm
    {
        //properties of GA
        private int populationSize = 100;
        private int elitismNumber = 1;
        private double mutationRate = 0.5;
        private double crossoverRate = 0.5;
        private int tournamentSize = 3;
        private Random random = new Random();

        public Population GenerateNextPopulation(Population currentpop)
        {
            Population crossoverPop = Crossover(currentpop);
            Population mutationPop = Mutation(crossoverPop);
            return mutationPop;
        }

        public Population Crossover(Population pop)
        {
            Population outputpop = new Population(pop.size);

            for (int i = 0; i < elitismNumber; i++)
            {
                outputpop.schedules.Add(pop.schedules[i]);
            }
            for (int j = elitismNumber; j < pop.size; j++)
            {
                Schedule parent_1 = Tournament(pop).schedules[0];
                Schedule parent_2 = Tournament(pop).schedules[0];

                Schedule outputschedule = CrossoverSchedule(parent_1, parent_2);
                outputpop.schedules.Add(outputschedule);

            }
            outputpop.schedules.ForEach(x => x.CalculateFitness());
            return outputpop;
        }

        public Schedule CrossoverSchedule(Schedule parent1, Schedule parent2)
        {
            Schedule child = new Schedule();
            for (int i = 0; i < parent1.classes.Count; i++)
            {
                if (random.NextDouble() > crossoverRate)
                {
                    child.classes.Add(parent1.classes[i]);
                }
                else
                {
                    child.classes.Add(parent2.classes[i]);
                }
            }

            return child;
        }


        public Population Mutation(Population pop)
        {
            for (int i = elitismNumber; i < pop.size; i++)
            {
                MutationSchedule(pop.schedules[i]);
            }
            return pop; 
        }

        public Schedule MutationSchedule(Schedule schedule)
        {
            Schedule newSchedule = new Schedule();
            newSchedule.InitializeSchedule();
            newSchedule.GetFitness();
            for (int i = 0; i < newSchedule.classes.Count; i++)
            {
                if (random.NextDouble() > mutationRate )
                {
                    schedule.classes[i] = newSchedule.classes[i];
                }
            }
            return schedule;
        }

        public Population Tournament(Population pop)
        {
            Population participant = new Population(tournamentSize);
            for (int i = 0; i < tournamentSize; i++)
            {
                Schedule candidate = pop.schedules[random.Next(pop.schedules.Count)];
                participant.schedules.Add(candidate);
            }

            participant.schedules.ForEach(x => x.CalculateFitness());
            participant.schedules = participant.schedules.OrderByDescending(x => x.fitnessValue).ToList();

            return participant;
        }

        public List<Class> Execute()
        {
            int GenerationNumber = 0;
            Population pop = new Population(populationSize);
            pop.InitializePopulation();
            pop.schedules = pop.schedules.OrderByDescending(x => x.fitnessValue).ToList();  

            while (pop.schedules[0].fitnessValue < 1.0)
            {
                GenerationNumber += 1;
                pop = GenerateNextPopulation(pop);
                pop.schedules.ForEach(x => x.GetFitness());
                pop.schedules = pop.schedules.OrderByDescending(x => x.fitnessValue).ToList();
            }

            return pop.schedules[0].classes;
        }
    }

}