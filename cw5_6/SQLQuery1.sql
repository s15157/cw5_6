DELETE from studies;
Select * from student;
select * from enrollment;
delete  from Enrollment
delete from student;
select * from studies
INSERT INTO Studies(IdStudy,Name) VALUES (3,'ARC');

SELECT COUNT(*)+1 FROM Enrollment;
select IdEnrollment from Enrollment where IdStudy=1 AND Semester = 1
INSERT INTO Enrollment(IdEnrollment,Semester,IdStudy,StartDate) VALUES ((SELECT COUNT(*)+1 FROM Enrollment),1,1,'2020-04-20T00:00:00+02:00')
select COUNT(*) from Student where IndexNumber='s15157'