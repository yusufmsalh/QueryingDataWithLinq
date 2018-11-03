using System;
using System.Linq;
using System.Security.Cryptography;

namespace Queries
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbcontext = new PlutoContext();

            #region Select one Column

            var q0 = dbcontext.Courses.Select(e => e.Name).FirstOrDefault();


            #endregion
            #region Restriction :
            //Simple
            var q1_simpleCondition = dbcontext.Courses.Where(e => e.Level == 1);

            //GetCourses with Name C#
            var q = from c in dbcontext.Courses
                    where c.Name.Contains("C#")
                    select c.Name;

            var q1 = dbcontext.Courses.Where(e => e.Name.Contains("C#")).Select(e => e.Name);
            #endregion
            #region Multiple Conditions

            var q2 = from c in dbcontext.Courses
                     where c.Name.Contains("C#") && c.FullPrice < 100
                     select c.Name;

            var q3 = dbcontext.Courses
                    .Where(e => e.Name.Contains("C#") && e.FullPrice < 100)
                    .Select(e => e.Name);

            #endregion
            #region Order By
            var q4 = from c in dbcontext.Courses
                     where c.Name.Contains("C#") && c.FullPrice < 100//you replace with ||
                     orderby c.FullPrice descending
                     select c.Name;

            var q5 = dbcontext.Courses
                .Where(e => e.Name.Contains("C#") && e.FullPrice < 100)
                .OrderByDescending(e => e.FullPrice)
                .Select(e => e.Name);
            #endregion
            #region Multiple Order By
            var q6 = from c in dbcontext.Courses
                     where c.Name.Contains("C#") && c.FullPrice < 100
                     orderby c.FullPrice descending, c.Author //by default ascending
                     select c.Name;

            var q7 = dbcontext.Courses
                .Where(e => e.Name.Contains("C#") && e.FullPrice < 100)
                .OrderByDescending(e => e.FullPrice)
                .ThenBy(e => e.Author)
                .Select(e => e.Name);
            #endregion
            #region Projection into new Anonymous Object 

            var q8 = from c in dbcontext.Courses
                     where c.Name.Contains("C#") && c.FullPrice < 100 //you replace with ||
                     orderby c.FullPrice descending, c.Author //by default ascending
                     select new
                     {
                         MyCourseName = c.Name,
                         MyCoursePrice = c.FullPrice

                     };

            var q9 = dbcontext.Courses
                .Where(e => e.Name.Contains("C#") && e.FullPrice < 100)
                .OrderByDescending(e => e.FullPrice)
                .OrderBy(e => e.Author)
                .Select(e => new
                {
                    MyCourseName = e.Name,
                    MyCoursePrice = e.FullPrice
                });
            #endregion
            #region Grouping

            var q10 = from c in dbcontext.Courses
                      group c by c.Level
                        into g
                      select g;

            var q11 = dbcontext.Courses.GroupBy(c => c.Level);
            foreach (var item in q11)
            {
                Console.WriteLine(item.Key);
                foreach (var course in item)
                {
                 //   Console.WriteLine(course.Name);
                }
            }

            #endregion
            #region Getting Data From Multiple Tables using Navigation Property
            //
            var q12 = from c in dbcontext.Courses
                      select new
                      {
                          CourseName = c.Name,
                          AuthorName = c.Author.Name

                      };
            var q13 = dbcontext.Courses.Select(c =>
                new
                {
                    CourseName = c.Name,
                    AuthorName = c.Author.Name

                }
            );
            #endregion
            #region Getting Data From Multiple Tables using Inner Join

            var q14 = from c in dbcontext.Courses
                      join a in dbcontext.Authors
                      on c.AuthorId equals a.Id
                      select new
                      {
                          CourseName = c.Name,
                          AuthorName = c.Author.Name
                      };

            var q15 = dbcontext.Courses.Join(dbcontext.Authors,
             c => c.AuthorId,
             a => a.Id,
             (c, a) => new
             {
                 AuthorName = a.Name,
                 CourseName = c.Name
             });
            foreach (var item in q14)
            {
                //   Console.WriteLine("Author name {0},Course Name {1}", item.AuthorName, item.CourseName);
            }

            #endregion
            #region Group Join
            var q16 = from a in dbcontext.Authors
                      join c in dbcontext.Courses
                      on a.Id equals c.AuthorId
                      into g
                      select new
                      {
                          AuthorName = a.Name,
                          CourseList = g
                      };

            var _q16 = dbcontext.Authors.GroupJoin(dbcontext.Courses, e => e.Id, c => c.AuthorId, (Author, Course) =>
                new
                {
                    AuthorName = Author.Name,
                    CourseList = Course
                });

            foreach (var item in q16)
            {
                // Console.WriteLine("Author Name {0},Count of Courses {1},List of Course :",item.AuthorName,item.CourseList.Count());
                foreach (var course in item.CourseList)
                {
                    //  Console.WriteLine("\t" + course.Name);
                }
            }
            #endregion
            #region Cross Join

            var q17 = from c in dbcontext.Courses
                      from a in dbcontext.Authors
                      select new
                      {
                          AuthorName = a.Name,
                          CourseList = c.Name
                      };

            var _q17 = dbcontext.Authors// n authors * m courses
                .SelectMany(e => dbcontext.Courses, (author, course) => new
            {
                AuthorName = author,
                CourseName = course
            });
            #endregion
            #region Select Vs Select Many
            //var q018 = dbcontext.Courses.Select(e => e.X);//return List of X
            //var q019 = dbcontext.Courses.SelectMany(e => e.X);//return list of Tags

            var q18 = dbcontext.Courses.Select(e => e.Tags);//return List of List of Tags
            var q19 = dbcontext.Courses.SelectMany(e => e.Tags);//return list of Tags

            var q20 = dbcontext.Courses.Select(e => e.Name);//return List of Name
            var q21 = dbcontext.Courses.SelectMany(e => e.Name);//return List of Chars : flattened Name



            #endregion
            #region Paging

            var q22 = dbcontext.Courses.Skip(10).Take(10);
            #endregion
            #region Element Operator : return specific element

            #region Single Vs SingleOrDefault

          var q23 = dbcontext.Courses.Single(c => c.Id == 0);//throw exception as sequence contains no elements.
            var q24 = dbcontext.Courses.SingleOrDefault(c => c.Id == 0);//returns null
                                                                        //      var q25 = dbcontext.Courses.SingleOrDefault(c => c.Name.Contains("C#"));//throws exception as sequence contains multiple elements.

            /*
             * Single :             if found fine ,if not found =>exception
             * SingleOrDefault :    if 0 result =>default (null) ,if 1 result =>fine,if multiple result => exception
             */

            #endregion

            #region First Vs FirstOrDefault
            var q25 = dbcontext.Courses.First(c => c.Id == 0);//throw exception if sequence is null or empty.
            var q26 = dbcontext.Courses.FirstOrDefault(c => c.Id == 0);//returns null
            #endregion
            #endregion

            #region Quantifying
            bool allAbove150UsdAll = dbcontext.Courses.All(e => e.FullPrice > 150);
            bool anyCourseHasFooInName = dbcontext.Courses.Any(e => e.Name.Contains("Fo"));
            #endregion

            #region Aggregation
            var cheapCoursesCount = dbcontext.Courses.Count(e => e.FullPrice < 50);
            var maxCoursePrice = dbcontext.Courses.Max(e => e.FullPrice);
            var minCoursePrice = dbcontext.Courses.Min(e => e.FullPrice);
            var avgCoursePrice = dbcontext.Courses.Average(e => e.FullPrice);
            #endregion

            Console.ReadKey();


        }
    }
}
