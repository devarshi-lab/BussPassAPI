using BussPassAPI.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BussPassAPI.Controllers
{
    [RoutePrefix("BusPass")]
    public class HomeController : ApiController
    {
        public FirebaseDB firebaseDB = new FirebaseDB("https://buspass-6d459-default-rtdb.firebaseio.com/");
        

        [HttpGet, Route("Get")]
        public string Get()
        {
            FirebaseDB firebaseDBTeams = firebaseDB.Node("Teams");
            FirebaseResponse getResponse = firebaseDBTeams.Get();
            
            if (getResponse.Success)
                return getResponse.JSONContent;
            
            return "Not Data Found";
        }

        [HttpGet, Route("GetWithId")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost, Route("Post")]
        public string Post()
        {
            var data = @"{  
                            'Team-Awesome': {  
                                'Members': {  
                                    'M1': {  
                                        'City': 'Hyderabad',  
                                        'Name': 'Ashish'  
                                        },  
                                    'M2': {  
                                        'City': 'Cyberabad',  
                                        'Name': 'Vivek'  
                                        },  
                                    'M3': {  
                                        'City': 'Secunderabad',  
                                        'Name': 'Pradeep'  
                                        }  
                                   }  
                               }  
                          }";
            FirebaseDB firebaseDBTeams = firebaseDB.Node("Teams");
            FirebaseResponse postResponse = firebaseDBTeams.Post(data);
            if (postResponse.Success)
                return "success";
            return "fail";
        }

        [HttpPut, Route("Put")]
        public string Put()
        {
            var data = @"{  
                            'Team-Awesome': {  
                                'Members': {  
                                    'M1': {  
                                        'City': 'Hyderabad',  
                                        'Name': 'Ashish'  
                                        },  
                                    'M2': {  
                                        'City': 'Cyberabad',  
                                        'Name': 'Vivek'  
                                        },  
                                    'M3': {  
                                        'City': 'Secunderabad',  
                                        'Name': 'Pradeep'  
                                        }  
                                   }  
                               }  
                          }";
            FirebaseDB firebaseDBTeams = firebaseDB.Node("Teams");
            FirebaseResponse putResponse = firebaseDBTeams.Put(data);
            if (putResponse.Success)
                return "success";
            return "fail";
        }

        [HttpPatch, Route("Patch")]
        public string Patch()
        {
            var data = @"{  
                            'Team-Awesome': {  
                                'Members': {  
                                    'M1': {  
                                        'City': 'Hyderabad',  
                                        'Name': 'Ashish'  
                                        },  
                                    'M2': {  
                                        'City': 'Cyberabad',  
                                        'Name': 'Vivek'  
                                        },  
                                    'M3': {  
                                        'City': 'Secunderabad',  
                                        'Name': 'Pradeep'  
                                        }  
                                   }  
                               }  
                          }";
            FirebaseDB firebaseDBTeams = firebaseDB.Node("Teams");
            FirebaseResponse patchResponse = firebaseDBTeams
                // Use of NodePath to refer path lnager than a single Node  
                .NodePath("Team-Awesome/Members/M1")
                .Patch("{\"Designation\":\"CRM Consultant\"}");
            if (patchResponse.Success)
                return "success";
            return "fail";
        }

        [HttpDelete, Route("Delete")]
        public string Delete()
        {
            var data = @"{  
                            'Team-Awesome': {  
                                'Members': {  
                                    'M1': {  
                                        'City': 'Hyderabad',  
                                        'Name': 'Ashish'  
                                        },  
                                    'M2': {  
                                        'City': 'Cyberabad',  
                                        'Name': 'Vivek'  
                                        },  
                                    'M3': {  
                                        'City': 'Secunderabad',  
                                        'Name': 'Pradeep'  
                                        }  
                                   }  
                               }  
                          }";
            FirebaseDB firebaseDBTeams = firebaseDB.Node("Teams");
            FirebaseResponse deleteResponse = firebaseDBTeams.Delete();
            if (deleteResponse.Success)
                return "success";
            return "fail";
        }
    }
}
