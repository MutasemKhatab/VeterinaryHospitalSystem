using Vet.Models;

namespace Vet.DataAccess.Repository.IRepository;

public interface ICaseStudyRepository : IRepository<CaseStudy>{
	void Update (CaseStudy caseStudy);
}