// DAL "Database Access Layers
// All classes in DAL should be only access to DB
// All classes in DAL should only be accessibele from BLL

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlissBase.Model;

namespace BlissBase.DAL
{
    /// <summary>
    /// CompositeScoreDAL Used to access the CompositeScore table
    /// This table keeps scores for composite symbols depending on
    /// there usage frequency. This table is ment to be used by the  
    /// ControlledVocabularyBLL to enable "autocomplete"
    /// </summary>
    public class CompositeScoreDAL
    {
        bool testing;
        public CompositeScoreDAL()
        {
            testing = false;
        }
        public CompositeScoreDAL(bool testing_)
        {
            testing = testing_;
        }

        /// <summary>
        /// Returns score for a compositeSymbol by id as int
        /// Returns 0 if no score exists for given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>score as int or 0</returns>
        public int GetScoreFor(int id)
        {
            using (var db = new BlissBaseContext(testing))
            {
                int score;
                try
                {
                    score = db.CompositeScores.Find(id).CompScore;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("A exception was thrown when checking score for CompositeSymbol: " + id +
                        " 'symbol might not have a score set yet!', check stacktrace");
                    Debug.WriteLine(e.StackTrace);
                    return 0;
                }
                return score;
            }
        }

        /// <summary>
        /// Updates score for given id and returns new score.
        /// Sets difference as score if no score is found for the id,
        /// and returns difference
        /// </summary>
        /// <param name="id"></param>
        /// <param name="difference"></param>
        /// <returns>Score after update as int</returns>
        public int UpdateScoreFor(int id, int difference)
        {
            using (var db = new BlissBaseContext(testing))
            {
                try
                {
                    CompositeScores toEdit = db.CompositeScores.Find(id);
                    if (toEdit == null)
                    {
                        toEdit = new CompositeScores
                        {
                            CompID = id,
                            CompScore = 0
                        };
                        toEdit.CompScore += difference;
                        db.CompositeScores.Add(toEdit);
                    }
                    else
                    {
                        toEdit.CompScore += difference;
                    }
                    
                    db.SaveChanges();
                    return toEdit.CompScore;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Ecxeption when updating score for id: " + id);
                    Debug.WriteLine(e.StackTrace);
                    return 0;
                }
            }
        }

        /// <summary>
        /// Deletes the row in the CompositScores table
        /// that corresponds to the given id. 
        /// Returns true if successfull, else false
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true or false depending on success</returns>
        public bool RemoveScoreFor(int id)
        {
            using (var db = new BlissBaseContext(testing))
            {
                try
                {
                    var toRemove = db.CompositeScores.Find(id);
                    db.CompositeScores.Remove(toRemove);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception when trying to delete score for id: " + id);
                    Debug.WriteLine(e.StackTrace);
                    return false;
                }
            }
        }
    }
}
