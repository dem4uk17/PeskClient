using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using Plesk.Schemas.Input;
using Plesk.Schemas.Output;

namespace PleskClient
{
    public class PleskClient
    {
        string Hostname;
        string Username;
        string Password;
        string ApiURL;

        public PleskClient(string hostname, string username, string password, int port = 8443)
        {
            this.Hostname = hostname;
            this.Username = username;
            this.Password = password;

            this.ApiURL = String.Format("https://{0}:{1}/enterprise/control/agent.php", Hostname, port);
        }

        private string SerializeObject<T>(T obj)
        {
            string xmlData;
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            xmlSerializer.Serialize(memoryStream, obj);
            xmlData = encoding.GetString(memoryStream.ToArray());

            return xmlData;
        }

        private ResponsePacketType DeserializeObject(string xml)
        {
            System.Xml.Serialization.XmlSerializer xmlSerializer = new XmlSerializer(typeof(ResponsePacketType));
            using (System.IO.StringReader reader = new StringReader(xml))
            {
                ResponsePacketType obj = (ResponsePacketType)xmlSerializer.Deserialize(reader);
                return obj;
            }
        }

        private string SendHttpRequest(string message)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
            (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
            {
                if (sslPolicyErrors != SslPolicyErrors.RemoteCertificateNotAvailable)
                    return true;

                // Do not allow this client to communicate with unauthenticated servers.
                return false;
            };

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("HTTP_AUTH_LOGIN", Username);
                client.Headers.Add("HTTP_AUTH_PASSWD", Password);
                client.Headers.Add("Content-Type", "text/xml");

                return client.UploadString(ApiURL, message);
            };
        }

        private ResponsePacketType ExecuteWebEequest(packet input)
        {
            var message = SerializeObject(input);
            var response = SendHttpRequest(message);
            return DeserializeObject(response);

        }

        public ResponsePacketType GetWebspaces()
        {
            packet packet = new packet();
            packet.ItemsElementName = new ItemsChoiceType76[] { ItemsChoiceType76.webspace };
            packet.Items = new Plesk.Schemas.Input.DomainTypeRequest[]
            {
                new Plesk.Schemas.Input.DomainTypeRequest()
                {
                    ItemsElementName = new Plesk.Schemas.Input.ItemsChoiceType13[] { Plesk.Schemas.Input.ItemsChoiceType13.get },
                    Items = new Plesk.Schemas.Input.DomainTypeRequestGet[]
                    {
                        new Plesk.Schemas.Input.DomainTypeRequestGet()
                        {
                            filter = new Plesk.Schemas.Input.domainFilterType(),
                            dataset = new Plesk.Schemas.Input.domainDatasetType()
                        }
                    }
                }
            };

            return ExecuteWebEequest(packet);
        }

        public ResponsePacketType GetWebspaces(string name)
        {
            packet packet = new packet();
            packet.ItemsElementName = new ItemsChoiceType76[] { ItemsChoiceType76.webspace };
            packet.Items = new Plesk.Schemas.Input.DomainTypeRequest[]
            {
                new Plesk.Schemas.Input.DomainTypeRequest()
                {
                    ItemsElementName = new Plesk.Schemas.Input.ItemsChoiceType13[] { Plesk.Schemas.Input.ItemsChoiceType13.get },
                    Items = new Plesk.Schemas.Input.DomainTypeRequestGet[]
                    {
                        new Plesk.Schemas.Input.DomainTypeRequestGet()
                        {
                            filter = new Plesk.Schemas.Input.domainFilterType()
                            {
                               ItemsElementName = new Plesk.Schemas.Input.ItemsChoiceType11[] { Plesk.Schemas.Input.ItemsChoiceType11.name },
                               Items = new string[] { name }
                            },

                            dataset = new Plesk.Schemas.Input.domainDatasetType()
                        }
                    }
                }
            };

            return ExecuteWebEequest(packet);
        }

        public ResponsePacketType CreateWebspace(string name, string ip)
        {
            packet packet = new packet();
            packet.ItemsElementName = new Plesk.Schemas.Input.ItemsChoiceType76[] { Plesk.Schemas.Input.ItemsChoiceType76.webspace };
            packet.Items = new Plesk.Schemas.Input.DomainTypeRequest[]
            {
                new Plesk.Schemas.Input.DomainTypeRequest
                {
                    ItemsElementName = new Plesk.Schemas.Input.ItemsChoiceType13[] { Plesk.Schemas.Input.ItemsChoiceType13.add },

                    Items = new Plesk.Schemas.Input.DomainTypeRequestAdd[]
                    {
                        new Plesk.Schemas.Input.DomainTypeRequestAdd
                        {
                            gen_setup = new Plesk.Schemas.Input.DomainTypeRequestAddGen_setup
                            {
                                ItemsElementName = new Plesk.Schemas.Input.ItemsChoiceType12[]
                                {
                                    Plesk.Schemas.Input.ItemsChoiceType12.name,
                                    Plesk.Schemas.Input.ItemsChoiceType12.ip_address
                                },
                                Items = new string[]
                                {
                                    name,
                                    ip
                                }
                            }
                        }
                    }
                }
            };

            return ExecuteWebEequest(packet);
        }

        public ResponsePacketType DeleteWebspace(int id)
        {
            packet packet = new packet();
            packet.ItemsElementName = new Plesk.Schemas.Input.ItemsChoiceType76[] { Plesk.Schemas.Input.ItemsChoiceType76.webspace };
            packet.Items = new Plesk.Schemas.Input.DomainTypeRequest[]
            {
                new Plesk.Schemas.Input.DomainTypeRequest
                {
                    ItemsElementName = new Plesk.Schemas.Input.ItemsChoiceType13[] { Plesk.Schemas.Input.ItemsChoiceType13.del },

                    Items = new Plesk.Schemas.Input.DomainTypeRequestDel[]
                    {
                        new Plesk.Schemas.Input.DomainTypeRequestDel
                        {
                            filter = new Plesk.Schemas.Input.domainFilterType
                            {
                                ItemsElementName = new Plesk.Schemas.Input.ItemsChoiceType11[] { Plesk.Schemas.Input.ItemsChoiceType11.id },
                                Items = new string[] { id.ToString() }
                            }
                        }
                    }
                }
            };

            return ExecuteWebEequest(packet);
        }

        public ResponsePacketType GetDomain(string name)
        {
            packet packet = new packet();
            packet.ItemsElementName = new Plesk.Schemas.Input.ItemsChoiceType76[] { Plesk.Schemas.Input.ItemsChoiceType76.site };
            packet.Items = new SiteTypeRequest[]
            {
                new SiteTypeRequest()
                {
                    Items = new SiteTypeRequestGet[]
                    {
                        new SiteTypeRequestGet()
                        {
                            filter = new Plesk.Schemas.Input.siteFilterType()
                            {
                                ItemsElementName = new Plesk.Schemas.Input.ItemsChoiceType63[] { Plesk.Schemas.Input.ItemsChoiceType63.name },
                                Items = new string[] { name }
                            },
                            dataset = new Plesk.Schemas.Input.siteDatasetType()
                        }
                    }
                }
            };

            return ExecuteWebEequest(packet);    
        }

        public ResponsePacketType GetMailAccounts(int siteId)
        {
            packet packet = new packet();
            packet.ItemsElementName = new Plesk.Schemas.Input.ItemsChoiceType76[] { Plesk.Schemas.Input.ItemsChoiceType76.mail };
            packet.Items = new MailTypeRequest[]
            {
                new MailTypeRequest()
                {
                    Items = new MailTypeRequestGet_info[]
                    {
                        new MailTypeRequestGet_info()
                        {
                            filter = new GetInfoAdvancedFilter() { siteid = siteId.ToString() },
                            mailboxusage = new Plesk.Schemas.Input.NoneType(),
                            mailbox = new Plesk.Schemas.Input.NoneType()
                        }
                    }
                }
            };
            
            return ExecuteWebEequest(packet);
        }

        public ResponsePacketType CreateMailAccount(int siteId, string name, string password, long quota = 0)
        {
            packet packet = new packet();
            packet.ItemsElementName = new Plesk.Schemas.Input.ItemsChoiceType76[] { Plesk.Schemas.Input.ItemsChoiceType76.mail };
            packet.Items = new MailTypeRequest[]
            {
                new MailTypeRequest()
                {
                    Items = new MailTypeRequestCreate[]
                    {
                        new MailTypeRequestCreate()
                        {
                            filter = new mailnameFilterAddType()
                            {
                                mailname = new Plesk.Schemas.Input.mailnameAddType[] 
                                {   
                                    new Plesk.Schemas.Input.mailnameAddType()
                                    {
                                        name = name,
                                        password = new Plesk.Schemas.Input.mailnameAddTypePassword()
                                        {
                                            type = Plesk.Schemas.Input.passwdType.plain,
                                            value = password
                                        },
                                        mailbox = quota > 0 ? new Plesk.Schemas.Input.mailnameAddTypeMailbox() {enabled = true, quota = quota, quotaSpecified = true } : null
                                        
                                    }
                                },
                                siteid = siteId.ToString()
                            }
                        }
                    }
                }
            };

            return ExecuteWebEequest(packet);
        }

        public ResponsePacketType UpdateMailAccount(int siteId, string name, string password, long quota = 0)
        {
            packet packet = new packet();
            packet.ItemsElementName = new Plesk.Schemas.Input.ItemsChoiceType76[] { Plesk.Schemas.Input.ItemsChoiceType76.mail };
            packet.Items = new MailTypeRequest[]
            {
                new MailTypeRequest()
                {
                    Items = new MailTypeRequestUpdate[]
                    {
                        new MailTypeRequestUpdate()
                        {
                            Item = new MailTypeRequestUpdateSet
                            {
                                filter = new mailnameFilterType()
                                {
                                    mailname = new Plesk.Schemas.Input.mailnameUpdateType[]
                                    {
                                        new Plesk.Schemas.Input.mailnameUpdateType()
                                        {
                                            name = name,
                                            mailbox = quota > 0 ? new Plesk.Schemas.Input.mailnameUpdateTypeMailbox() {enabled = true, quota = quota, quotaSpecified = true } : null,
                                            Item = new Plesk.Schemas.Input.mailnameUpdateTypePassword()
                                            {
                                                type = Plesk.Schemas.Input.passwdType.plain,
                                                value = password,
                                                typeSpecified = true
                                            }
                                        }
                                    },

                                    siteid = siteId.ToString()
                                }
                            }
                        }
                    }
                }
            };

            return ExecuteWebEequest(packet);
        }

        public ResponsePacketType DeleteMailAccount(int siteId, string name)
        {
            packet packet = new packet();
            packet.ItemsElementName = new Plesk.Schemas.Input.ItemsChoiceType76[] { Plesk.Schemas.Input.ItemsChoiceType76.mail };
            packet.Items = new MailTypeRequest[]
            {
                new MailTypeRequest()
                {
                    Items = new MailTypeRequestRemove[]
                    {
                        new MailTypeRequestRemove()
                        {
                            filter = new GetInfoAdvancedFilter()
                            {
                                siteid = siteId.ToString(),
                                name = new string[] { name }
                            }
                        }
                    }
                }
            };

            return ExecuteWebEequest(packet);
        }
    }
}