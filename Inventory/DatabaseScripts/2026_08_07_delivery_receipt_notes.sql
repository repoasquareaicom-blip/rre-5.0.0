The analysis is approved.

Do NOT implement yet.

Before implementation, perform one final focused analysis of the EXISTING sync infrastructure only:

1. ProductMasterCloudQueue.cs
2. ProductCloudSyncClient.cs
3. Tool/ProductSyncQueue.cs
4. BranchSyncService or any existing ProductMaster upsert implementation
5. Existing ProductMasterCloudQueue database table/schema
6. Existing App.config branch API URL keys

For each, report the exact current behavior.

Most importantly determine:

- What columns currently exist in ProductMasterCloudQueue.
- Whether one ProductId currently creates only one queue row.
- How target API URL is currently selected.
- What JSON payload ProductCloudSyncClient sends.
- What API endpoint it currently calls.
- Whether Add, Edit and Price changes already use the same payload.
- How retry currently works.
- How success/failure is recorded.
- Whether a successful queue row is retained or deleted.
- Whether ProductSyncQueue already has a Push Again/Retry action.
- How BranchSyncService performs ProductMaster upsert.
- Whether remote upsert preserves the Salem ProductId exactly.
- Whether identity insert is involved.
- What happens if the ProductId already exists remotely.
- What happens if ItemName/ItemCode already exists with a different ProductId.
- What App.config keys currently exist for Salem/Namakkal/Kolathur API URLs.

Then propose the MINIMUM modifications needed to evolve the existing system into:

Salem local save
    -> queue RR-NAMAKKAL
    -> queue RR-KOLATHUR
    -> attempt both independently

Required queue behavior:

ProductId 100
RR-NAMAKKAL -> SUCCESS
RR-KOLATHUR -> FAILED

must remain independently traceable.

Only the failed Kolathur entry should require retry.

Also analyze how Price Upload with many products should be queued efficiently. We do not want the UI frozen while hundreds of remote API calls execute.

Do not modify code.
Do not modify SQL.
Do not modify App.config.
Do not create migrations.

Return the proposed final implementation plan and exact files/database changes required.

We will approve that plan before implementation.

IMPORTANT ADDITIONAL BUSINESS RULE:

Before production rollout, ProductMaster at RR-NAMAKKAL and RR-KOLATHUR will be reloaded/synchronized from the Salem ProductMaster.

Therefore Salem is the authoritative/master source for ProductMaster.

Namakkal and Kolathur ProductMaster should be treated as mirrors of Salem ProductMaster.

ProductId must remain exactly the same across all three branch databases.

When Salem creates ProductId 100, remote branches must also use ProductId 100. Do not generate a new ProductId at Namakkal or Kolathur.

Future Product Add/Edit and Price Upload operations will originate only from RR-SALEM and will then be propagated to the other branches.

Take this into account when analyzing the existing ProductMaster upsert, identity handling, duplicate handling, and sync queue design.

Still ANALYSIS ONLY. Do not make any code or database changes yet.