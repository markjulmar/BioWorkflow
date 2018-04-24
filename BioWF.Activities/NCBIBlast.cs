using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Bio;
using Bio.Web;
using Bio.Web.Blast;
using System.Threading;
using Bio.Extensions;
using System.Threading.Tasks;

namespace BioWF.Activities
{
    /// <summary>
    /// Activity to issue a BLAST request to NCBI
    /// </summary>
    public sealed class NCBIBlast : AsyncCodeActivity<string>
    {
        public const string DefaultProgram = "blastn";
        public const string DefaultDatabase = "nr";

        /// <summary>
        /// Sequences to BLAST
        /// </summary>
        [RequiredArgument]
        [Category(ActivityConstants.InputGroup)]
        [Description("Sequences to pass to BLAST.")]
        public InArgument<IEnumerable<ISequence>> Sequences { get; set; }

        /// <summary>
        /// The program to use - defaults to blastn
        /// </summary>
        [DefaultValue(DefaultProgram)]
        [Category(ActivityConstants.InputGroup)]
        [Description("The program to use - defaults to 'blastn'.")]
        public InArgument<string> Program { get; set; }

        /// <summary>
        /// The database to use.
        /// </summary>
        [DefaultValue(DefaultDatabase)]
        [Category(ActivityConstants.InputGroup)]
        [Description("The database to use - defaults to 'nr'.")]
        public InArgument<string> Database { get; set; }

        /// <summary>
        /// Whether to use a browser proxy
        /// </summary>
        [Category(ActivityConstants.InputGroup)]
        [Description("Whether to use a browser proxy")]
        public bool UseBrowserProxy { get; set; }

        /// <summary>
        /// Gets the Xml Serialized data from the given stream.
        /// </summary>
        /// <param name="stream">memory stream</param>
        /// <returns>serialized blast string</returns>
        private static string GetSerializedData(Stream stream)
        {
            string xml = string.Empty;
            var memStream = stream as MemoryStream;
            if (memStream != null)
            {
                xml = Encoding.UTF8.GetString(memStream.GetBuffer());
                xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
                xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
                memStream.Close();
            }
            return xml;
        }

        /// <summary>
        /// When implemented in a derived class and using the specified execution context, callback method, and user state, enqueues an asynchronous activity in a run-time workflow. 
        /// </summary>
        /// <returns>
        /// An object.
        /// </returns>
        /// <param name="context">Information that defines the execution environment for the <see cref="T:System.Activities.AsyncCodeActivity"/>.</param><param name="callback">The method to be called after the asynchronous activity and completion notification have occurred.</param><param name="state">An object that saves variable information for an instance of an asynchronous activity.</param>
        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            NcbiBlastWebHandler service = new NcbiBlastWebHandler();

            // fill in the BLAST settings:
            var searchParams = new BlastRequestParameters
            {
                Program = Program.Get(context) ?? DefaultProgram,
                Database = Database.Get(context) ?? DefaultDatabase
            };

            searchParams.ExtraParameters.Add("Expect", "1e-10");
            searchParams.Sequences.AddRange(Sequences.Get(context));

            // Create the request
            var runningTask = service.ExecuteAsync(searchParams, CancellationToken.None);

            context.UserState = runningTask;
            return runningTask.AsApm(callback, state);
        }

        /// <summary>
        /// When implemented in a derived class and using the specified execution environment information, notifies the workflow runtime that the associated asynchronous activity operation has completed.
        /// </summary>
        /// <returns>
        /// A generic type.
        /// </returns>
        /// <param name="context">Information that defines the execution environment for the <see cref="T:System.Activities.AsyncCodeActivity"/>.</param>
        /// <param name="asyncResult">The implemented <see cref="T:System.IAsyncResult"/> that returns the status of an asynchronous activity when execution ends.</param>
        protected override string EndExecute(AsyncCodeActivityContext context, IAsyncResult asyncResult)
        {
            var runningTask = context.UserState as Task<Stream>;

            // Get blast result.
            BlastXmlParser blastParser = new BlastXmlParser();
            IList<BlastResult> results = blastParser.Parse(runningTask.Result).ToList();

            // Convert blast result to BlastCollator.
            List<BlastResultCollator> blastResultCollator = new List<BlastResultCollator>();
            foreach (BlastResult result in results)
            {
                foreach (BlastSearchRecord record in result.Records)
                {
                    if (record.Hits != null && 0 < record.Hits.Count)
                    {
                        foreach (Hit hit in record.Hits.Where(hit => hit.Hsps != null && 0 < hit.Hsps.Count))
                        {
                            blastResultCollator.AddRange(hit.Hsps.Select(hsp =>
                                new BlastResultCollator
                                {
                                    Alignment = hsp.AlignmentLength,
                                    Bit = hsp.BitScore,
                                    EValue = hsp.EValue,
                                    Identity = hsp.IdentitiesCount,
                                    Length = hit.Length,
                                    QEnd = hsp.QueryEnd,
                                    QStart = hsp.QueryStart,
                                    QueryId = record.IterationQueryId,
                                    SEnd = hsp.HitEnd,
                                    SStart = hsp.HitStart,
                                    SubjectId = hit.Id,
                                    Positives = hsp.PositivesCount,
                                    QueryString = hsp.QuerySequence,
                                    SubjectString = hsp.HitSequence,
                                    Accession = hit.Accession,
                                    Description = hit.Def
                                }));
                        }
                    }
                }
            }

            BlastXmlSerializer serializer = new BlastXmlSerializer();
            Stream stream = serializer.SerializeBlastOutput(blastResultCollator);

            // set result to the output property.
            return GetSerializedData(stream);

        }
    }

    /// <summary>
    /// BlastResultCollater class collates all the information of the
    /// Blast search for UI rendering.
    /// </summary>
    public class BlastResultCollator
    {
        /// <summary>
        /// Gets or sets the Query Id of the BLAST search result.
        /// </summary>
        public string QueryId { get; set; }

        /// <summary>
        /// Gets or sets the Subject Id of the BLAST search result.
        /// </summary>
        public string SubjectId { get; set; }

        /// <summary>
        /// Gets or sets the Identity of the BLAST search result.
        /// </summary>
        public long Identity { get; set; }

        /// <summary>
        /// Gets or sets the Alignment found in the BLAST search result.
        /// </summary>
        public long Alignment { get; set; }

        /// <summary>
        /// Gets or sets the Length of the BLAST search result.
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// Gets or sets the MisMatches found in the BLAST search result.
        /// </summary>
        public string Mismatches { get; set; }

        /// <summary>
        /// Gets or sets the GapOpenings found in the BLAST search result.
        /// </summary>
        public string GapOpenings { get; set; }

        /// <summary>
        /// Gets or sets the QStartof the BLAST search result.
        /// </summary>
        public long QStart { get; set; }

        /// <summary>
        /// Gets or sets the QEnd the BLAST search result.
        /// </summary>
        public long QEnd { get; set; }

        /// <summary>
        /// Gets or sets the SStart the BLAST search result.
        /// </summary>
        public long SStart { get; set; }

        /// <summary>
        /// Gets or sets the SEnd the BLAST search result.
        /// </summary>
        public long SEnd { get; set; }

        /// <summary>
        /// Gets or sets the EValue the BLAST search result.
        /// </summary>
        public double EValue { get; set; }

        /// <summary>
        /// Gets or sets the Number of bits in the BLAST search result.
        /// </summary>
        public double Bit { get; set; }

        /// <summary>
        /// Gets or sets the Number of positives in the BLAST search result.
        /// </summary>
        public long Positives { get; set; }

        /// <summary>
        /// Gets or sets the Query string of the BLAST search result.
        /// </summary>
        public string QueryString { get; set; }

        /// <summary>
        /// Gets or sets the Subject string of the BLAST search result.
        /// </summary>
        public string SubjectString { get; set; }

        /// <summary>
        /// Gets or sets the Accession number of the hit
        /// </summary>
        public string Accession { get; set; }

        /// <summary>
        /// Gets or sets the Description of the hit
        /// </summary>
        public string Description { get; set; }
    }

    public class BlastXmlSerializer
    {
        /// <summary>
        /// Gets the serializer type used for serialization
        /// </summary>
        public string SerializerType { get; private set; }

        /// <summary>
        /// This method would serialize the blast result 
        /// and return the serialized stream.
        /// </summary>
        /// <param name="result">Blast Result</param>
        /// <returns>Serialized stream</returns>
        public Stream SerializeBlastOutput(IList<BlastResultCollator> result)
        {
            XmlTextWriter xmlWriter = null;
            MemoryStream memStream = null;
            try
            {
                this.SerializerType = "XmlSerializer";
                XmlSerializer serializer = new XmlSerializer(typeof(List<BlastResultCollator>));
                memStream = new MemoryStream();
                xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);
                serializer.Serialize(xmlWriter, result);
                return memStream;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            finally
            {
                if (xmlWriter != null && memStream != null)
                {
                    xmlWriter.Close();
                    memStream.Close();
                }
            }
        }
    }
}
