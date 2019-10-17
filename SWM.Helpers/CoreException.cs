using System;
using Microsoft.Data.SqlClient;
using System.Net;
using System.Text.RegularExpressions;

namespace SWM.Helpers
{
    public class CoreException : Exception
    {
        readonly string _message;
        public override string Message => _message;
        const string ExistsPattern = "{0} with '{1}' '{2}' already exists.";
        const string NotExistsPattern = "{0} with given {1} does not exist.";
        public int Code { get; set; }
        public CoreException(string msg = "Internal server error", HttpStatusCode code = HttpStatusCode.InternalServerError)
        {
            _message = msg;
            Code = (int)code;
        }

        public CoreException(Exception e)
        {

            if (e is SqlException ex)
            {

                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    Code = (int)HttpStatusCode.Conflict;
                    Regex re = new Regex(@"ADM.(\w+).*_(\w+).* \((.*)\)");
                    if (re.IsMatch(ex.Message))
                    {
                        string schema = re.Match(ex.Message).Groups[1].ToString();
                        string constraint = re.Match(ex.Message).Groups[2].ToString();
                        string value = re.Match(ex.Message).Groups[3].ToString();
                        _message = string.Format(ExistsPattern, schema.Remove(schema.Length - 1), constraint, value);
                    }
                    else return;
                }

                if (ex.Number == 547)
                {
                    Code = (int)HttpStatusCode.NotFound;
                    Regex re = new Regex(@"ADM.(\w+).*\'(.*)\'");
                    if (re.IsMatch(ex.Message))
                    {
                        string schema = re.Match(ex.Message).Groups[1].ToString();
                        string constraint = re.Match(ex.Message).Groups[2].ToString();
                        _message = string.Format(NotExistsPattern, schema.Remove(schema.Length - 1), constraint);
                    }
                }
            }
        }
    }
}
