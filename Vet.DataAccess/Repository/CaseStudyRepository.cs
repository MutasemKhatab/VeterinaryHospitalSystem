using Vet.DataAccess.Data;
using Vet.DataAccess.Repository.IRepository;
using Vet.Models;

namespace Vet.DataAccess.Repository;

public class CaseStudyRepository(AuthDbContext db) :Repository<CaseStudy>(db),ICaseStudyRepository {
	public void Update(CaseStudy caseStudy) {
		db.CaseStudies.Update(caseStudy);
	}
}