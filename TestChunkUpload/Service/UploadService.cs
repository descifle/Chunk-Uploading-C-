using ChunkUpload.Data;

namespace ChunkUpload.Service
{
    public class UploadService
    {
        private const int CHUNK_LIMIT = 1024 * 1024;

        Dictionary<String, Session> sessions;
        FileRepository fileStorage;

        public UploadService(FileRepository storage)
        {
            this.fileStorage = storage;
            sessions = new Dictionary<String, Session>();
        }

        public Session createSession(long user, String fileName, int chunkSize, long fileSize)
        {

            if (String.IsNullOrWhiteSpace(fileName))
                throw new Exception("File name missing");



            if (chunkSize > CHUNK_LIMIT)
                throw new Exception(String.Format("Maximum chunk size is {0} bytes", CHUNK_LIMIT));

            if (chunkSize < 1)
                throw new Exception("Chunk size must be greater than zero");

            if (fileSize < 1)
                throw new Exception("Total size must be greater than zero");

            Session session = new Session(user, new FileInformation(fileSize, fileName, chunkSize));
            sessions.Add(session.Id, session);


            return session;
        }

        public Session getSession(String id)
        {
            return sessions[id];
        }

        public List<Session> getAllSessions()
        {
            return sessions.Values.ToList();
        }

        public void persistBlock(String sessionId, long userId, int chunkNumber, byte[] buffer)
        {
            Session session = getSession(sessionId);

            try
            {
                if (session == null)
                {
                    throw new Exception("Session not found");
                }

                fileStorage.Persist(sessionId, chunkNumber, buffer);



                session.FileInfo.MarkChunkAsPersisted(chunkNumber);
                session.RenewTimeout();
            }
            catch (System.Exception e)
            {
                if (session != null)
                    session.MaskAsFailed();

                throw e;
            }
        }

        public void UploadFileToServer(Session session)
        {
            fileStorage.RecreateFile(session.Id, session.FileInfo);
        }

        public void WriteToStream(Stream stream, Session session)
        {
            fileStorage.WriteToStream(stream, session);
        }

        public Stream GetFileStream(Session session) {
            return fileStorage.GetFileStream(session);
        }
    }
}