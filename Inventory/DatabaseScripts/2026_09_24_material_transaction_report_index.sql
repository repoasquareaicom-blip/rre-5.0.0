/*
  Optional index for the Material Transaction Report.
  Not applied automatically.

  MaterialTranscation is a heap. The report filters TranscationDate and MaterailId
  and aggregates IN/OUT. This index keeps opening-stock and period scans off the heap
  as the table grows. It does not change posted data.

  Review, then run manually if report speed becomes a problem:

CREATE NONCLUSTERED INDEX IX_MaterialTranscation_Report
    ON dbo.MaterialTranscation (MaterailId, TranscationDate)
    INCLUDE (Type, Quantity, LocationId, RackId, TranscationType, TransId, Updatedby);
*/