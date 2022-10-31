using System.Collections.Specialized;

using Microsoft.SqlServer.Management.Smo;

namespace Script
{
    class JobScript : SqlScript
    {
        #region Private Members

        //private Microsoft.SqlServer.Management.Smo.Agent.Job j;
        private string _scriptCreate;

        #endregion Private Members

        #region Public Constructors

        public JobScript()
        {
        }

        //public JobScript(string jobName, string scriptCreate, string jobCategory)
        //{
        //    JobName = jobName;
        //    _scriptCreate = scriptCreate;
        //    JobCategory = jobCategory;
        //}
        public JobScript(Microsoft.SqlServer.Management.Smo.Agent.Job job)
        {
            JobName = job.Name;
            JobCategory = job.Category;
            _scriptCreate = ScriptCreate(job.Script(_scriptingOptions));
        }

        #endregion Public Constructors

        #region Public Properties

        public string JobCategory
        {
            get; set;
        }

        public string JobName
        {
            get; set;
        }

        #endregion Public Properties

        #region Public Static Methods

        public static string ScriptGet(string jobName)
        {
            string sql_job_date_modified = "select date_modified from [msdb].[dbo].[sysjobs] where [name] = '{0}'";
            return string.Format(sql_job_date_modified, jobName);
        }

        #endregion Public Static Methods

        #region Public Override Methods

        public override string ScriptCreate()
        {
            return _scriptCreate;
        }

        public override string ScriptDisable()
        {
            string jobName = SafeSql(JobName);
            string script = @"
PRINT '-- DISABLE JOB : {0}';
IF  EXISTS (SELECT job_id FROM msdb.dbo.sysjobs_view WHERE name = N'{0}')
begin
    EXEC msdb.dbo.sp_update_job @job_name = N'{0}', @enabled = 0;
end;
ELSE
BEGIN
    print '-- Job does not exist : {0}';
END;
            " + Line1;
            return script;
        }

        public override string ScriptDrop()
        {
            string jobName = SafeSql(JobName);
            string script = @"
PRINT '-- DROP JOB : {0}';
IF  EXISTS (SELECT job_id FROM msdb.dbo.sysjobs_view WHERE name = N'{0}')
begin
    EXEC msdb.dbo.sp_delete_job @job_name=N'{0}' , @delete_unused_schedule=1;
end;
ELSE
BEGIN
    print '** Job does not exist : {0}';
END;
            " + Line1;
            script = string.Format(script, jobName);
            return script;
        }

        public override string ScriptEnable()
        {
            string jobName = SafeSql(JobName);
            string script = @"
PRINT '-- ENABLE JOB : " + jobName + @"';
IF  EXISTS (SELECT job_id FROM msdb.dbo.sysjobs_view WHERE name = N'{0}')
begin
    EXEC msdb.dbo.sp_update_job @job_name = N'{0}', @enabled = 1;
end;
ELSE
BEGIN
    print '** Job does not exist : {0}';
END;
            " + Line1;
            script = string.Format(script, jobName);
            return script;
        }

        #endregion Public Override Methods

        #region Public Methods

        public string ScriptGetServer(bool getPrimary)
        {
            string typeServer = getPrimary ? "PRIMARY" : "SECONDARY";
            string script = Line1 + @"
IF SERVERPROPERTY ('IsHadrEnabled') = 1
BEGIN
SELECT TOP 1 sys.dm_hadr_availability_replica_cluster_states.replica_server_name
, sys.availability_groups_cluster.name
, sys.dm_hadr_availability_replica_states.role_desc
, sys.availability_group_listeners.dns_name
FROM       sys.availability_groups_cluster
INNER JOIN sys.dm_hadr_availability_replica_cluster_states ON sys.dm_hadr_availability_replica_cluster_states.group_id = sys.availability_groups_cluster.group_id
INNER JOIN sys.dm_hadr_availability_replica_states         ON sys.dm_hadr_availability_replica_states.replica_id = sys.dm_hadr_availability_replica_cluster_states.replica_id
INNER JOIN sys.availability_group_listeners                ON sys.availability_group_listeners.group_id = sys.dm_hadr_availability_replica_states.group_id
WHERE
(sys.dm_hadr_availability_replica_states.role_desc = '{0}')
END
ELSE
BEGIN
SELECT
    '' as name -- Availability Group
, '' as replica_server_name -- SQL cluster node name
, '' as role_desc -- Replica Role
, '' as dns_name -- Listener Name
END;
            ";
            script = string.Format(script, typeServer);

            return script;
        }

        #endregion Public Methods

        #region Private Methods

        private string ScriptCreate(StringCollection stringCollection)
        {
            string jobName = SafeSql(JobName);
            string script = @"
PRINT '-- CREATE JOB : " + jobName + @"';
IF  NOT EXISTS (SELECT job_id FROM msdb.dbo.sysjobs_view WHERE name = N'{1}')
BEGIN
    {0}
END
ELSE
BEGIN
    print '-- Job exists and cannot be re-created : {1}';
END;
            ";
            script = string.Format(script, StringCollectionToString(stringCollection), jobName);
            return  script;
        }

        #endregion Private Methods
    }
}