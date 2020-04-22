CREATE PROCEDURE PromoteStudents @Studies NVARCHAR(100), @Semester INT
AS
BEGIN
	SET XACT_ABORT ON;
	BEGIN TRAN

	DECLARE @IdStudy INT = (SELECT IdStudy FROM Studies WHERE Name=@Studies);
	IF @IdStudy IS NULL
	BEGIN
		
		RAISERROR ('Brak podanych studiów',16,1);

		RETURN;
	END

	DECLARE @IdEnrollment INT = (SELECT IdEnrollment FROM Enrollment WHERE IdStudy = @IdStudy AND Semester = @Semester+1);
	IF @IdEnrollment IS NULL
	BEGIN
		INSERT INTO Enrollment(IdEnrollment,Semester,IdStudy,StartDate) VALUES ((SELECT COUNT(*)+1 FROM Enrollment), @Semester+1, @IdStudy, GETDATE());
		SET @IdEnrollment = (SELECT IdEnrollment FROM Enrollment WHERE IdStudy = @IdStudy AND Semester = @Semester+1);
	END

	DECLARE @IdEnrollmentOld INT = (SELECT IdEnrollment FROM Enrollment WHERE IdStudy = @IdStudy AND Semester = @Semester);

	UPDATE Student
	SET IdEnrollment = @IdEnrollment
	WHERE IdEnrollment = @IdEnrollmentOld;
	
	COMMIT
END;